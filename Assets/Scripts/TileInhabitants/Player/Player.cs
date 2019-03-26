using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Player : ITurnTaker, IDamageable {
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

  private PlayerSubEntity TopLeft => entities[dim - 1, 0];
  private readonly PlayerSubEntity[,] entities = new PlayerSubEntity[dim, dim]; //Indexed by [row, col]
  private readonly PlayerObject gameObject;

  private PlayerStates State { get; set; }

  private int XWallJumpPower => State.HasFlag(PlayerStates.RightWallSliding) ? -gameObject.xWallJumpPower : State.HasFlag(PlayerStates.LeftWallSliding) ? gameObject.xWallJumpPower : 0;
  private int XAcceleration => IsGrounded ? gameObject.xAccelerationGrounded : gameObject.xAccelerationAerial;
  private int turnsStunned = 0;

  private int jumpGraceTurns = 0;

  public int Row => TopLeft.Row;
  public int Col => TopLeft.Col;

  public bool IsInIllegalPosition { get; private set; }


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
    private set => _yVelocity = Mathf.Clamp(value, -gameObject.maxFallSpeed, gameObject.maxRiseSpeed);
  }
  public bool IsGrounded => State.HasFlag(PlayerStates.Grounded);
  public bool IsWallSliding => gameObject.wallJumpingEnabled && (State.HasFlag(PlayerStates.LeftWallSliding) || State.HasFlag(PlayerStates.RightWallSliding));
  public bool IsStunned => turnsStunned > 0;
  public bool IsDroppingThroughPlatform => State.HasFlag(PlayerStates.DroppingThroughPlatform);


  //
  //Methods
  //

  public Player(PlayerObject gameObject) {
    this.gameObject = gameObject;
    for (int r = 0; r < dim; r++) {
      for (int c = 0; c < dim; c++) {
        SingleTileEntityObject subentityGameObject = new GameObject().AddComponent<SingleTileEntityObject>();
        subentityGameObject.name = string.Format("Player[r={0}, c={1}]", r, c); ;
        subentityGameObject.spawnRow = gameObject.spawnRow + r;
        subentityGameObject.spawnCol = gameObject.spawnCol + c;
        subentityGameObject.transform.parent = gameObject.transform;
        PlayerSubEntity newEntity = new PlayerSubEntity(subentityGameObject, this, out bool success);
        if (!success) {
          Destroy();
          throw new System.Exception(string.Format("Failed to create PlayerSubEntity at row {0}, col {1}", gameObject.spawnRow + r, gameObject.spawnCol + c));
        }
        entities[r, c] = newEntity;
      }
    }

    //Parent graphics to the TopLeft
    Vector3 relativePosition = gameObject.graphicsHolder.transform.position;
    gameObject.graphicsHolder.transform.parent = TopLeft.gameObject.transform;
    gameObject.graphicsHolder.transform.localPosition = relativePosition;

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
    IsInIllegalPosition = false;
    foreach (PlayerSubEntity entity in entities) {
      if (!entity.CanSetPosition(entity.Row, entity.Col)) {
        YVelocity = 1;
        XVelocity = 0;
        IsInIllegalPosition = true;
      }
    }

    bool isGroundedAtStartOfTurn = IsGrounded;

    //Adjust fall speed based on presence of updrafts
    int originalMaxFallSpeed = gameObject.maxFallSpeed;
    foreach (PlayerSubEntity entity in entities) {
      if (entity.InUpdraft) {
        gameObject.maxFallSpeed = 1;
      }
    }

    //If the player is stunned, disregard their input
    if (turnsStunned > 0) {
      turnsStunned -= 1;
      selectedAction = Action.Wait;
    }

    //Apply knockback to the player from the previous turn
    //If there isn't any, respond the player's input instead
    if (knockback != null) {
      XVelocity = 0;
      YVelocity = 0;
      if (knockback.Value == Direction.East || knockback.Value == Direction.West) {
        PerformMove(Direction.North);
      }
      for (int i = 0; i < 3; i++) {
        if (!PerformMove(knockback.Value)) {
          break;
        }
      }
    } else {
      PerformAction(selectedAction);
      Attack();
    }

    knockback = null;
    gameObject.maxFallSpeed = originalMaxFallSpeed;

    if (jumpGraceTurns > 0) {
      jumpGraceTurns -= 1;
    }

    if (!IsGrounded && isGroundedAtStartOfTurn) {
      jumpGraceTurns = gameObject.jumpGraceTurns;
    }

    if (selectedAction == Action.Jump) {
      jumpGraceTurns = 0;
    }
  }

  public ISet<Tile> Occupies() {
    ISet<Tile> result = new HashSet<Tile>();
    foreach (PlayerSubEntity entity in entities) {
      result.UnionWith(entity.Occupies());
    }
    return result;
  }


  //
  //Attacking
  //

  //Attack enemies below self
  private void Attack() {
    foreach (PlayerSubEntity bottom in Bottom()) {
      Tile t = GameManager.S.Board.GetInDirection(bottom.Row, bottom.Col, Direction.South);
      if (t != null) {
        foreach (ITileInhabitant below in t.Inhabitants) {
          if (!(below is IDamageable)) {
            continue;
          }
          IDamageable victim = (IDamageable)below;
          if (CanAttack(victim)) {
            victim.OnAttacked(gameObject.attackPower, Direction.South);
          }
        }
      }
    }
  }

  private bool CanAttack(IDamageable other) {
    return other is IEnemy;
  }


  //
  //Performing actions
  //

  private void PerformAction(Action action) {
    switch (action) {
      case Action.Jump: JumpAction(); break;
      case Action.Drop: DropAction(); break;
      case Action.MoveLeft: MoveLeftAction(); break;
      case Action.MoveRight: MoveRightAction(); break;
      case Action.Wait: WaitAction(); break;
      default: throw new System.ArgumentException("Illegal enum value detected");
    }

    if (XVelocity != 0 || YVelocity != 0) {
      List<Vector2Int> moveWaypoints = TopLeft.CalculateMoveWaypoints(XVelocity, YVelocity);

      //Skip the first waypoint because it's just our current position
      bool xCollisionFlag = false;
      bool yCollisionFlag = false;
      for (int i = 1; i < moveWaypoints.Count; i++) {
        Vector2Int waypoint = moveWaypoints[i];
        if (yCollisionFlag) {
          waypoint.y = TopLeft.Row;
        }
        if (xCollisionFlag) {
          waypoint.x = TopLeft.Col;
        }

        //Guaranteed that exactly one of these is +/- 1.
        int xDir = waypoint.x - TopLeft.Col;
        int yDir = waypoint.y - TopLeft.Row;
        if (xDir == 0 && yDir == 0) {
          continue;
        }
        Direction moveDirection = 
          xDir == 1 ? Direction.East : 
          xDir == -1 ? Direction.West : 
          yDir == 1 ? Direction.North : 
          yDir == -1 ? Direction.South : 
          throw new System.Exception("Illegal waypoint");

        bool moveSuccessful = PerformMove(moveDirection);
        if (!moveSuccessful) {
          if (yDir != 0) {
            yCollisionFlag = true;
          }
          if (xDir != 0) {
            xCollisionFlag = true;
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

    if (IsWallSliding && YVelocity < -gameObject.wallSlideSpeed) {
      YVelocity = -gameObject.wallSlideSpeed;
    }

    if (IsDroppingThroughPlatform) {
      State &= ~PlayerStates.DroppingThroughPlatform;
      foreach (PlayerSubEntity entity in entities) {
        if (!entity.CanSetPosition(entity.Row, entity.Col)) {
          State |= PlayerStates.DroppingThroughPlatform;
          return;
        }
      }
    }
  }

  private void JumpAction() {
    if (IsWallSliding) {
      YVelocity = gameObject.yWallJumpPower;
      XVelocity = XWallJumpPower;
      SoundManager.S.PlayerJump();
    } else {
      YVelocity = gameObject.jumpPower;
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

  private void MoveLeftAction() {
    State &= ~PlayerStates.RightWallSliding;
    State &= ~PlayerStates.LeftWallSliding;

    if (IsGrounded && XVelocity >= gameObject.skidAndTurnThreshold) {
      State |= PlayerStates.SkidTurning; //Set skid flag
      XVelocity = gameObject.skidSpeed;
    } else {
      if (XVelocity > 0) {
        XVelocity = 0;
      }
      XVelocity -= XAcceleration;
    }
  }

  private void MoveRightAction() {
    State &= ~PlayerStates.RightWallSliding;
    State &= ~PlayerStates.LeftWallSliding;

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
  //Performing a move
  //

  //Attempts to move one space in direction.  If the movement fails, calls OnCollision to update state.
  private bool PerformMove(Direction moveDirection) {
    int colDelta = moveDirection == Direction.East ? 1 : moveDirection == Direction.West ? -1 : 0;
    int rowDelta = moveDirection == Direction.North ? 1 : moveDirection == Direction.South ? -1 : 0;

    //Check that every subentity can make the move
    foreach (PlayerSubEntity p in entities) {
      int newRow = p.Row + rowDelta;
      int newCol = p.Col + colDelta;
      if (!p.CanSetPosition(newRow, newCol)) {
        OnCollision(moveDirection);
        return false;
      }
    }

    //Move all the subentities and the gameObject
    foreach (PlayerSubEntity p in entities) {
      int newRow = p.Row + rowDelta;
      int newCol = p.Col + colDelta;

      p.SetPosition(newRow, newCol, out bool enteredNewPosition);
      if (!enteredNewPosition) {
        throw new System.Exception("Unexpected failure in SetPosition");
      }
    }
    return true;
  }

  //Respond to a collision when moving in moveDirection
  private void OnCollision(Direction moveDirection) {
    switch (moveDirection) {
      case Direction.North:
        YVelocity = 0;
        SoundManager.S.PlayerHeadBonk();
        break;

      case Direction.South:
        YVelocity = 0;
        SoundManager.S.PlayerLanded();
        break;

      case Direction.East:
        if (!IsGrounded && selectedAction == Action.MoveRight) {
          State |= PlayerStates.RightWallSliding;
        }
        XVelocity = 0;
        break;

      case Direction.West:
        if (!IsGrounded && selectedAction == Action.MoveLeft) {
          State |= PlayerStates.LeftWallSliding;
        }
        XVelocity = 0;
        break;

      default:
        throw new System.ArgumentException("Invalid enum value");
    }
  }


  //
  //Action selection
  //

  private Action selectedAction = Action.Wait;

  public bool CanSelectAction(Action action) {
    if (action == Action.Jump && !(IsGrounded || IsWallSliding || jumpGraceTurns > 0)) {
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

  private Direction? knockback = null;
  public void OnAttacked(int attackPower, Direction attackDirection) {
    SoundManager.S.PlayerDamaged();
    knockback = attackDirection;
    if (turnsStunned <= 0) {
      turnsStunned = attackPower;
    }
  }

  public void Destroy() {
    foreach (PlayerSubEntity entity in entities) {
      entity.Destroy();
    }
    GameManager.S.UnregisterTurnTaker(this);
    Object.Destroy(gameObject.gameObject);
  }

  public bool SubEntityIsBottom(PlayerSubEntity entity) {
    foreach (PlayerSubEntity other in Bottom()) {
      if (other == entity) {
        return true;
      }
    }
    return false;
  }


  //
  //Side iterators (for use in foreach loops)
  //

  private IEnumerable<PlayerSubEntity> Bottom() {
    for (int c = 0; c < dim; c++) {
      yield return entities[0, c];
    }
  }
  //private IEnumerable<PlayerSubEntity> Left() {
  //  for (int r = 0; r < dim; r++) {
  //    yield return entities[0, r];
  //  }
  //}
  //private IEnumerable<PlayerSubEntity> Right() {
  //  for (int r = 0; r < dim; r++) {
  //    yield return entities[dim - 1, r];
  //  }
  //}
}
