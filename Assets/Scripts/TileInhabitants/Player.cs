using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Player : SingleTileEntity, IActor, IDamageable, IAttacker {
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


  //
  //SingleTileEntity
  //

  public override bool IsBlockedBy(ITileInhabitant other) {
    return other is Wall;
  }

  public override void OnTurn() {
    PerformAction(selectedAction);
    selectedAction = Action.Wait;

    if (XVelocity != 0 || YVelocity != 0) {
      List<Vector2Int> moveWaypoints = CalculateMoveWaypoints(XVelocity, YVelocity);
      for (int i = 0; i < moveWaypoints.Count; i++) {
        Vector2Int waypoint = moveWaypoints[i];
        int newRow = waypoint.y;
        int newCol = waypoint.x;
        int xDir = newCol - Col;
        int yDir = newRow - Row;

        SetPosition(newRow, newCol, out bool enteredNewPosition);
        if (!enteredNewPosition) {
          //We couldn't enter the new position.  Must have encountered an obstacle.

          //Compute stuff like: 
          //  Changes to state (e.g. wall sliding)
          //  Changes to velocity (e.g. head-bonk)

          break;
        }
      }
    }

    //Clear skid flag
    State &= ~PlayerStates.SkidTurning;

    if (CheckForGround()) {
      State |= PlayerStates.Grounded;
    } else {
      State &= ~PlayerStates.Grounded;
    }

    //Apply gravity
    if (!State.HasFlag(PlayerStates.Grounded)) {
      YVelocity -= gravity;
    }
  }

  private bool CheckForGround() {
    return !GameManager.S.Board.IsPositionLegal(Row-1, Col);
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
          XVelocity -= XAcceleration;
        }
        break;

      case Action.MoveRight:
        if (IsGrounded && XVelocity <= -skidAndTurnThreshold) {
          State |= PlayerStates.SkidTurning; //Set skid flag
          XVelocity = -skidSpeed;
        } else {
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