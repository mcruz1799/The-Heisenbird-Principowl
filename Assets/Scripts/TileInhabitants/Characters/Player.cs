﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : ITurnTaker, IDamageable {
  [System.Flags] private enum PlayerStates {
    None = 0,
    Grounded = 1 << 0,
    LeftWallSliding = 1 << 1,
    RightWallSliding = 1 << 2,
    SkidTurning = 1 << 3,
    HeadBonking = 1 << 4,
    DroppingThroughPlatform = 1 << 5,
  }

  private static readonly int dim = 3;

  private PlayerSubEntity TopLeft => entities[0, dim - 1];
  private readonly PlayerSubEntity[,] entities = new PlayerSubEntity[dim, dim];
  private readonly PlayerObject gameObject;

  private PlayerStates State { get; set; }

  private bool IsDroppingThroughPlatform => State.HasFlag(PlayerStates.DroppingThroughPlatform);

  private int XWallJumpPower => State.HasFlag(PlayerStates.RightWallSliding) ? -gameObject.xWallJumpPower : State.HasFlag(PlayerStates.LeftWallSliding) ? gameObject.xWallJumpPower : 0;
  private int XAcceleration => IsGrounded ? gameObject.xAccelerationGrounded : gameObject.xAccelerationAerial;

  //These are for enemy targeting
  public int Row => TopLeft.Row;
  public int Col => TopLeft.Col;


  //
  //Variables exposed for animations
  //

  private int _xVelocity;
  public int XVelocity {
    get => _xVelocity;
    private set => _xVelocity = Mathf.Clamp(value, -gameObject.xSpeedMax, gameObject.xSpeedMax);
  }

  private int _yVelocity;
  public int YVelocity {
    get => _yVelocity;
    private set => _yVelocity = Mathf.Clamp(value, gameObject.ySpeedMin, gameObject.ySpeedMax);
  }
  public bool IsGrounded => State.HasFlag(PlayerStates.Grounded);
  public bool IsWallSliding => State.HasFlag(PlayerStates.LeftWallSliding) || State.HasFlag(PlayerStates.RightWallSliding);


  //
  //Methods
  //

  public Player(PlayerObject gameObject) {
    this.gameObject = gameObject;
    _damageable = new Damageable(gameObject.maxHp);
    for (int r = 0; r < dim; r++) {
      for (int c = 0; c < dim; c++) {
        SingleTileEntityObject subentityGameObject = new GameObject().AddComponent<SingleTileEntityObject>();
        subentityGameObject.name = string.Format("Player[r={0}, c={1}]", r, c); ;
        subentityGameObject.spawnRow = gameObject.spawnRow + r;
        subentityGameObject.spawnCol = gameObject.spawnCol + c;
        subentityGameObject.transform.parent = gameObject.transform;
        entities[c, r] = new PlayerSubEntity(subentityGameObject, this);
      }
    }

    //Parent graphics to the TopLeft
    gameObject.graphicsHolder.transform.parent = TopLeft.gameObject.transform;
    gameObject.graphicsHolder.transform.localPosition = Vector3.zero;

    //Sub entities shouldn't interact with each other
    foreach (PlayerSubEntity p in entities) {
      foreach (PlayerSubEntity other in entities) {
        if (other == p) continue;
        p.toIgnore.Add(other);
      }
    }

    PerformAction(Action.Wait);
    GameManager.S.RegisterTurnTaker(this);
  }

  public void OnTurn() {
    PerformAction(selectedAction);
    Attack();
  }

  //Attack enemies below self
  private void Attack() {
    foreach (PlayerSubEntity bottom in Bottom()) {
      Tile t = GameManager.S.Board.GetInDirection(bottom.Row, bottom.Col, Direction.South);
      if (t != null) {
        foreach (ITileInhabitant victim in t.Inhabitants) {
          if (victim is IDamageable && bottom.CanAttack((IDamageable)victim)) {
            bottom.Attack((IDamageable)victim);
          }
        }
      }
    }
  }


  //
  //Performing actions
  //

  private void PerformAction(Action action) {
    switch (action) {
      case Action.Jump: JumpAction(); break;
      case Action.Drop: DropAction(); break;
      case Action.MoveLeft: MoveAction(-1); break;
      case Action.MoveRight: MoveAction(1); break;
      case Action.Wait: WaitAction(); break;
      default: throw new System.ArgumentException("Illegal enum value detected");
    }

    if (XVelocity != 0 || YVelocity != 0) {
      List<Vector2Int> moveWaypoints = TopLeft.CalculateMoveWaypoints(XVelocity, YVelocity);

      //Skip the first waypoint because it's just our current position
      for (int i = 1; i < moveWaypoints.Count; i++) {
        Vector2Int waypoint = moveWaypoints[i];

        //Guaranteed that exactly one of these is +/- 1.
        int xDir = waypoint.x - TopLeft.Col;
        int yDir = waypoint.y - TopLeft.Row;

        //Check that every subentity can make the move
        bool canMove = true;
        foreach (PlayerSubEntity p in entities) {
          int newRow = p.Row + yDir;
          int newCol = p.Col + xDir;
          if (!p.CanSetPosition(newRow, newCol)) {
            //We couldn't enter the new position.  Must have encountered an obstacle.

            if (yDir > 0) {
              YVelocity = 0;
              SoundManager.S.PlayerHeadBonk();

            } else if (yDir < 0) {
              YVelocity = 0;
              SoundManager.S.PlayerLanded();
            }

            //Wall sliding
            if (xDir != 0) {
              if (!IsGrounded) {
                State |= xDir < 0 ? PlayerStates.LeftWallSliding : PlayerStates.RightWallSliding;
                YVelocity = -gameObject.wallSlideSpeed;
              }
              XVelocity = 0;
            }

            canMove = false;
            break;
          }
        }

        //If a subentity couldn't make the move, exit the loop
        if (!canMove) {
          break;
        }

        //Move all the subentities and the gameObject
        foreach (PlayerSubEntity p in entities) {
          int newRow = p.Row + yDir;
          int newCol = p.Col + xDir;

          p.SetPosition(newRow, newCol, out bool enteredNewPosition);
          if (!enteredNewPosition) {
            throw new System.Exception("Unexpected failure in SetPosition");
          }
        }
      }
    }

    //Clear skid flag
    State &= ~PlayerStates.SkidTurning;

    //Clear dropping through platform flag
    State &= ~PlayerStates.DroppingThroughPlatform;

    //Update grounded flag
    State &= ~PlayerStates.Grounded;
    foreach (PlayerSubEntity p in Bottom()) {
      if (p.IsGrounded) {
        State |= PlayerStates.Grounded;
        break;
      }
    }

    //Apply gravity
    if (!IsGrounded) {
      YVelocity -= gameObject.gravity;
    }
  }

  private void JumpAction() {
    if (IsGrounded) {
      YVelocity = gameObject.jumpPower;
      SoundManager.S.PlayerJump();

    } else if (IsWallSliding) {
      YVelocity = gameObject.yWallJumpPower;
      XVelocity = XWallJumpPower;
      SoundManager.S.PlayerJump();
    }
    State &= ~PlayerStates.RightWallSliding;
    State &= ~PlayerStates.LeftWallSliding;
  }

  private void DropAction() {
    if (IsGrounded) {
      State |= PlayerStates.DroppingThroughPlatform;
      if (YVelocity == 0) {
        YVelocity = -1;
      }
    }
  }

  private void MoveAction(int xDir) {
    State &= ~PlayerStates.RightWallSliding;
    State &= ~PlayerStates.LeftWallSliding;

    if (xDir < 0) {
      if (IsGrounded && XVelocity >= gameObject.skidAndTurnThreshold) {
        State |= PlayerStates.SkidTurning; //Set skid flag
        XVelocity = gameObject.skidSpeed;
      } else {
        if (XVelocity > 0) {
          XVelocity = 0;
        }
        XVelocity -= XAcceleration;
      }
    } else if (xDir > 0) {
      if (IsGrounded && XVelocity <= -gameObject.skidAndTurnThreshold) {
        State |= PlayerStates.SkidTurning; //Set skid flag
        XVelocity = -gameObject.skidSpeed;
      } else {
        if (XVelocity < 0) {
          XVelocity = 0;
        }
        XVelocity += XAcceleration;
      }
    }
  }

  private void WaitAction() {
    if (XVelocity > 0) {
      XVelocity -= gameObject.xDeceleration;
      if (XVelocity < 0) {
        XVelocity = 0;
      }
    } else if (XVelocity < 0) {
      XVelocity += gameObject.xDeceleration;
      if (XVelocity > 0) {
        XVelocity = 0;
      }
    }
  }


  //
  //Action selection
  //

  private Action selectedAction = Action.Wait;

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

  private readonly Damageable _damageable;
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
      UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
  }


  //
  //Side iterators (for use in foreach loops)
  //

  private IEnumerable<PlayerSubEntity> Bottom() {
    for (int c = 0; c < dim; c++) {
      yield return entities[c, 0];
    }
  }
  private IEnumerable<PlayerSubEntity> Left() {
    for (int r = 0; r < dim; r++) {
      yield return entities[0, r];
    }
  }
  private IEnumerable<PlayerSubEntity> Right() {
    for (int r = 0; r < dim; r++) {
      yield return entities[dim - 1, r];
    }
  }
}
