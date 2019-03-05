using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Player : SingleTileEntity, IActor, ITurnTaker, IDamageable, IAttacker {
  [System.Flags] private enum PlayerStates {
    None = 0,
    Grounded = 1 << 0,
    LeftWallSliding = 1 << 1,
    RightWallSliding = 1 << 2,
    SkidTurning = 1 << 3,
    HeadBonking = 1 << 4,
    DroppingThroughPlatform =  1 << 5,
  }

  private readonly PlayerObject p;

  private PlayerStates State { get; set; } = PlayerStates.Grounded;
  private Action selectedAction = Action.Wait;

  private int _xVelocity;
  private int XVelocity {
    get => _xVelocity;
    set => _xVelocity = Mathf.Clamp(value, -p.xSpeedMax, p.xSpeedMax);
  }

  private int _yVelocity;
  private int YVelocity {
    get => _yVelocity;
    set => _yVelocity = Mathf.Clamp(value, p.ySpeedMin, p.ySpeedMax);
  }

  //Exposed for PlayerAnimator
  public bool IsGrounded => State.HasFlag(PlayerStates.Grounded);
  public bool IsWallSliding => State.HasFlag(PlayerStates.LeftWallSliding) || State.HasFlag(PlayerStates.RightWallSliding);

  private int XWallJumpPower => State.HasFlag(PlayerStates.RightWallSliding) ? -p._xWallJumpPower : State.HasFlag(PlayerStates.LeftWallSliding) ? p._xWallJumpPower : 0;
  private int XAcceleration => IsGrounded ? p._xAccelerationGrounded : p._xAccelerationAerial;

  public Player(PlayerObject p) : base(p) {
    this.p = p;
    _damageable = new Damageable(this.p._maxHp);
    GameManager.S.RegisterTurnTaker(this);
  }


  //
  //SingleTileEntity
  //

  protected override bool IsBlockedByCore(ITileInhabitant other) {
    if (other is Enemy) {
      return true;
    }

    if (other is Platform) {
      Platform platform = (Platform)other;
      if (!platform.IsActive) {
        return false;
      }
      if (platform.PlayerCanJumpThrough && platform.Row == Row+1) {
        return false;
      }
      if (State.HasFlag(PlayerStates.DroppingThroughPlatform) && platform.PlayerCanDropThrough && platform.Row == Row-1) {
        return false;
      }
      return true;
    }

    return false;
  }


  //
  //ITurnTaker
  //

  public void OnTurn() {
    PerformAction(selectedAction);
    //Note that selectedAction is reset to Wait at the end of this function

    if (XVelocity != 0 || YVelocity != 0) {
      List<Vector2Int> moveWaypoints = CalculateMoveWaypoints(XVelocity, YVelocity);

      //Skip the first waypoint because it's just our current position
      for (int i = 1; i < moveWaypoints.Count; i++) {
        Vector2Int waypoint = moveWaypoints[i];
        int newRow = waypoint.y;
        int newCol = waypoint.x;

        //Guaranteed that exactly one of these is +/- 1.
        int xDir = newCol - Col;
        int yDir = newRow - Row;

        SetPosition(newRow, newCol, out bool enteredNewPosition);
        if (!enteredNewPosition) {
          //We couldn't enter the new position.  Must have encountered an obstacle.

          if (yDir > 0) {
            //Debug.Log("Bonked head");
            YVelocity = 0;
            SoundManager.S.PlayerHeadBonk();

          } else if (yDir < 0) {
            YVelocity = 0;
            //Debug.Log("Landed");
            SoundManager.S.PlayerLanded();
          }

          if (xDir != 0) {
            if (!IsGrounded) {
              State |= xDir < 0 ? PlayerStates.LeftWallSliding : PlayerStates.RightWallSliding;
              YVelocity = -p.wallSlideSpeed;
            }
            XVelocity = 0;
          }

          break;
        }
      }
    }

    //Clear skid flag
    State &= ~PlayerStates.SkidTurning;

    //Clear dropping through platform flag
    State &= ~PlayerStates.DroppingThroughPlatform;

    //Update grounded flag
    if (CheckForGround()) {
      State |= PlayerStates.Grounded;
    } else {
      State &= ~PlayerStates.Grounded;
    }

    //Apply gravity
    if (!IsGrounded) {
      YVelocity -= p.gravity;
    }

    //Clear the selected action
    selectedAction = Action.Wait;
  }

  private bool CheckForGround() {
    //Can't fall out of bounds
    if (!GameManager.S.Board.IsPositionLegal(Row-1, Col)) {
      return true;
    }

    //If you can't be added to the Tile, it counts as ground.
    if (!GameManager.S.Board[Row - 1, Col].CanAdd(this)) {
      return true;
    }

    return false;
  }

  private void PerformAction(Action action) {
    switch (action) {
      case Action.Jump:
        if (IsGrounded) {
          YVelocity += p.jumpPower;
          SoundManager.S.PlayerJump();

        } else if (IsWallSliding) {
          YVelocity = p.yWallJumpPower;
          XVelocity = XWallJumpPower;
          SoundManager.S.PlayerJump();
        }
        State &= ~PlayerStates.RightWallSliding;
        State &= ~PlayerStates.LeftWallSliding;
        break;

      case Action.Drop:
        if (IsGrounded) {
          State |= PlayerStates.DroppingThroughPlatform;
          if (YVelocity == 0) {
            YVelocity = -1;
          }
        }
        break;

      case Action.MoveLeft:
        State &= ~PlayerStates.RightWallSliding;
        State &= ~PlayerStates.LeftWallSliding;

        if (IsGrounded && XVelocity >= p.skidAndTurnThreshold) {
          State |= PlayerStates.SkidTurning; //Set skid flag
          XVelocity = p.skidSpeed;
        } else {
          if (XVelocity > 0) {
            XVelocity = 0;
          }
          XVelocity -= XAcceleration;
        }
        break;

      case Action.MoveRight:
        State &= ~PlayerStates.RightWallSliding;
        State &= ~PlayerStates.LeftWallSliding;

        if (IsGrounded && XVelocity <= -p.skidAndTurnThreshold) {
          State |= PlayerStates.SkidTurning; //Set skid flag
          XVelocity = -p.skidSpeed;
        } else {
          if (XVelocity < 0) {
            XVelocity = 0;
          }
          XVelocity += XAcceleration;
        }
        break;

      case Action.Wait:
        if (XVelocity > 0) {
          XVelocity -= p.xDeceleration;
          if (XVelocity < 0) {
            XVelocity = 0;
          }
        } else if (XVelocity < 0) {
          XVelocity += p.xDeceleration;
          if (XVelocity > 0) {
            XVelocity = 0;
          }
        }
        break;

      default:
        Debug.LogError("Illegal enum value detected");
        break;
    }
  }


  //
  //IActor
  //

  public bool CanSelectAction(Action action) {
    if (action == Action.Jump && !(IsGrounded || IsWallSliding)) {
      return false;
    }

    return true;
  }

  public void SelectAction(Action action) {
    if (!CanSelectAction(action)) {
      Debug.LogError("Attempting to select an illegal action");
      return;
    }
    selectedAction = action;
  }


  //
  //IDamageable
  //

  private Damageable _damageable;
  public int MaxHitpoints => _damageable.MaxHitpoints;
  public int Hitpoints => _damageable.Hitpoints;
  public bool IsAlive => _damageable.IsAlive;

  public int CalculateDamage(IAttacker attacker, int baseDamage) {
    return _damageable.CalculateDamage(baseDamage);
  }

  public void TakeDamage(IAttacker attacker, int baseDamage) {
    _damageable.TakeDamage(baseDamage);
    if (_damageable.IsAlive) {
      SoundManager.S.PlayerDamaged();
    } else {
      SoundManager.S.PlayerDied();
    }
  }


  //
  //IAttacker
  //

  public bool CanAttack(IDamageable other) {
    return other is Enemy;
  }

  public void Attack(IDamageable other) {
    if (!CanAttack(other)) {
      Debug.LogError("Attempting an illegal attack");
      return;
    }
    other.TakeDamage(this, p._attackPower);
  }


  //
  //Other
  //

  private void UpdateSounds() {
    //SoundManager.S.ToggleWallSlidingSfx(IsWallSliding);
  }
}