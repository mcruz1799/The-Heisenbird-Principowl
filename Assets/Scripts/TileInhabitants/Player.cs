using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Player : SingleTileEntity, IActor, ITurnTaker, IDamageable, IAttacker {
  [System.Flags] private enum PlayerStates {
    None = 0,
    Grounded = 1 << 0,
    WallSliding = 1 << 1,
    SkidTurning = 1 << 2,
    HeadBonking = 1 << 3,
  }

#pragma warning disable 0649
  [Range(1, 99)] [SerializeField] private int gravity = 1;
  [Range(1, 99)] [SerializeField] private int jumpPower = 1;
  [Range(1, 99)] [SerializeField] private int _xAccelerationGrounded = 1;
  [Range(1, 99)] [SerializeField] private int _xAccelerationAerial = 1;
  [Range(1, 99)] [SerializeField] private int xDeceleration = 1;

  [Range(1, 99)] [SerializeField] private int xSpeedMax = 1;
  [Range(1, 99)] [SerializeField] private int ySpeedMax = 1;

  [Range(3, 99)] [SerializeField] private int skidAndTurnThreshold = 3;
  [Range(1,  2)] [SerializeField] private int skidSpeed = 1; //Must be less than skidAndTurnThreshold
#pragma warning restore 0649

  private PlayerStates State { get; set; } = PlayerStates.Grounded;
  private Action selectedAction = Action.Wait;

  private int _xVelocity;
  private int XVelocity {
    get => _xVelocity;
    set => _xVelocity = Mathf.Clamp(value, -xSpeedMax, xSpeedMax);
  }

  private int _yVelocity;
  private int YVelocity {
    get => _yVelocity;
    set => _yVelocity = Mathf.Clamp(value, -ySpeedMax, ySpeedMax);
  }

  public bool IsGrounded => State.HasFlag(PlayerStates.Grounded);
  private int XAcceleration => IsGrounded ? _xAccelerationGrounded : _xAccelerationAerial;

  private void Start() {
    GameManager.S.RegisterTurnTaker(this);
  }


  //
  //SingleTileEntity
  //

  public override bool IsBlockedBy(ITileInhabitant other) {
    bool isWall = other is Wall;
    return isWall;
  }


  //
  //ITurnTaker
  //

  public void OnTurn() {
    PerformAction(selectedAction);
    selectedAction = Action.Wait;

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
            Debug.Log("TODO: Bonked head");
          }

          if (yDir < 0) {
            YVelocity = 0;
            Debug.Log("TODO: Landed");
          }

          if (xDir != 0) {
            Debug.Log("TODO: Hit wall");
          }

          //Compute stuff like: 
          //  Changes to state (e.g. wall sliding)
          //  Changes to velocity (e.g. head-bonk)

          break;
        }
      }
    }

    //Clear skid flag
    State &= ~PlayerStates.SkidTurning;

    //Update grounded flag
    if (CheckForGround()) {
      State |= PlayerStates.Grounded;
    } else {
      State &= ~PlayerStates.Grounded;
    }

    //Apply gravity
    if (!IsGrounded) {
      YVelocity -= gravity;
    }
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
          YVelocity += jumpPower;
        }
        break;

      case Action.Drop:
        if (IsGrounded) {
          Debug.LogWarning("TODO: Drop-through platforms by pressing down");
        }
        break;

      case Action.MoveLeft:
        if (IsGrounded && XVelocity >= skidAndTurnThreshold) {
          State |= PlayerStates.SkidTurning; //Set skid flag
          XVelocity = skidSpeed;
        } else {
          if (XVelocity > 0) {
            XVelocity = 0;
          }
          XVelocity -= XAcceleration;
        }
        break;

      case Action.MoveRight:
        if (IsGrounded && XVelocity <= -skidAndTurnThreshold) {
          State |= PlayerStates.SkidTurning; //Set skid flag
          XVelocity = -skidSpeed;
        } else {
          if (XVelocity < 0) {
            XVelocity = 0;
          }
          XVelocity += XAcceleration;
        }
        break;

      case Action.Wait:
        if (XVelocity > 0) {
          XVelocity -= xDeceleration;
          if (XVelocity < 0) {
            XVelocity = 0;
          }
        } else if (XVelocity < 0) {
          XVelocity += xDeceleration;
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
    if (action == Action.Jump && !IsGrounded) {
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

  public int CalculateDamage(IAttacker attacker, int baseDamage) {
    throw new System.NotImplementedException();
  }

  public void TakeDamage(IAttacker attacker, int baseDamage) {
    throw new System.NotImplementedException();
  }


  //
  //IAttacker
  //

  public bool CanAttack(IDamageable other) {
    throw new System.NotImplementedException();
  }
}