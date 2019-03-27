using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy<TParent, TSub> : ITurnTaker, IDamageable, IEnemy
  where TParent : Enemy<TParent, TSub>
  where TSub : EnemySubEntity<TParent, TSub> {

  private readonly EnemyObject gameObject;
  private readonly TSub[,] entities; //[row, col] indexing
  private readonly int dim;
  protected TSub TopLeft => entities[dim - 1, 0];

  private int _xVelocity;
  public int XVelocity {
    get => _xVelocity;
    protected set => _xVelocity = Mathf.Clamp(value, -gameObject.xSpeedMax, gameObject.xSpeedMax);
  }

  private int _yVelocity;
  public int YVelocity {
    get => _yVelocity;
    protected set => _yVelocity = Mathf.Clamp(value, gameObject.ySpeedMin, gameObject.ySpeedMax);
  }

  public int AttackPower => gameObject.attackStunTurns;

  protected abstract void OnCollision(Direction moveDirection);
  protected abstract TSub CreateSubEntity(EnemyObject e, int spawnRow, int spawnCol, out bool success);

  public Enemy(EnemyObject gameObject, out bool success) {
    _damageable = new Damageable(gameObject.maxHp);
    this.gameObject = gameObject;
    dim = gameObject.dim;
    gameObject.graphicsHolder.enemy = this;

    if (dim < 1) {
      throw new System.ArgumentException("Dimension must be positive");
    }

    success = true;
    entities = new TSub[dim, dim];
    for (int r = 0; r < dim; r++) {
      for (int c = 0; c < dim; c++) {
        TSub subentity = CreateSubEntity(gameObject, gameObject.spawnRow + r, gameObject.spawnCol + c, out success);
        if (!success) {
          Destroy();
          return;
        }
        entities[r, c] = subentity;
      }
    }


    //Parent graphics to the TopLeft
    Vector3 relativePosition = gameObject.graphicsHolder.transform.position;
    gameObject.graphicsHolder.transform.parent = TopLeft.gameObject.transform;
    gameObject.graphicsHolder.transform.localPosition = relativePosition;

    //Sub entities shouldn't interact with each other
    foreach (SingleTileEntity entity in entities) {
      foreach (SingleTileEntity other in entities) {
        if (other == entity) continue;
        entity.toIgnore.Add(other);
      }
    }

    GameManager.S.RegisterTurnTaker(this);
  }

  protected abstract void OnTurnCore();

  public void OnTurn() {

    //Destroy self if dead
    if (!IsAlive) {
      Destroy();
      return;
    }

    //Destroy self if in illegal position
    foreach (TSub entity in entities) {
      if (!entity.CanSetPosition(entity.Row, entity.Col)) {
        Destroy();
        return;
      }
    }

    OnTurnCore();

    //Attack then move
    Attack();
    if (XVelocity != 0 || YVelocity != 0) {
      List<Vector2Int> moveWaypoints = TopLeft.CalculateMoveWaypoints(XVelocity, YVelocity);

      //Skip the first waypoint because it's just our current position
      bool yCollisionFlag = false;
      for (int i = 1; i < moveWaypoints.Count; i++) {
        Vector2Int waypoint = moveWaypoints[i];
        if (yCollisionFlag) {
          waypoint.y = TopLeft.Row;
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
          } else {
            break;
          }
        }
      }
    }
  }

  //Attempts to move one space in direction.  If the movement fails, calls OnCollision to update state.
  protected bool PerformMove(Direction moveDirection) {
    int colDelta = moveDirection == Direction.East ? 1 : moveDirection == Direction.West ? -1 : 0;
    int rowDelta = moveDirection == Direction.North ? 1 : moveDirection == Direction.South ? -1 : 0;

    //Check that every subentity can make the move
    foreach (TSub entity in entities) {
      int newRow = entity.Row + rowDelta;
      int newCol = entity.Col + colDelta;
      if (!entity.CanSetPosition(newRow, newCol)) {
        OnCollision(moveDirection);
        return false;
      }
    }

    //Move all the subentities and the gameObject
    foreach (TSub entity in entities) {
      int newRow = entity.Row + rowDelta;
      int newCol = entity.Col + colDelta;

      entity.SetPosition(newRow, newCol, out bool enteredNewPosition);
      if (!enteredNewPosition) {
        throw new System.Exception("Unexpected failure in SetPosition");
      }
    }
    return true;
  }


  //
  //IDamageable
  //

  private Damageable _damageable;
  public int MaxHitpoints => _damageable.MaxHitpoints;
  public int Hitpoints => _damageable.Hitpoints;
  public bool IsAlive => _damageable.IsAlive;

  public virtual void OnAttacked(int attackPower, Direction attackDirection) {
    _damageable.TakeDamage(_damageable.CalculateDamage(attackPower));
  }

  private void Attack() {
    foreach (TSub entity in entities) {
      entity.Attack();
    }
  }

  public virtual void Destroy() {
    foreach (SingleTileEntity entity in entities) {
      if (entity != null) {
        entity.Destroy();
      }
    }
    GameManager.S.UnregisterTurnTaker(this);
    Object.Destroy(gameObject.gameObject);
  }

  protected IEnumerable<TSub> Bottom() {
    for (int c = 0; c < dim; c++) {
      yield return entities[0, c];
    }
  }
}