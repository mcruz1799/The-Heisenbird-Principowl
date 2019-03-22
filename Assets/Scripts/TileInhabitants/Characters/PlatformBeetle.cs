using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeetleSubEntity : EnemySubEntity<PlatformBeetle, PlatformBeetleSubEntity> {
  private Direction AttackDirection => parent.XVelocity > 0 ? Direction.East : Direction.West;

  public PlatformBeetleSubEntity(SingleTileEntityObject gameObject, PlatformBeetle parent) : base(gameObject, parent) {
  }

  public override void Attack() {
    Tile t = GameManager.S.Board.GetInDirection(Row, Col, AttackDirection);
    if (t != null) {
      foreach (ITileInhabitant other in t.Inhabitants) {
        IDamageable victim = other is IDamageable ? (IDamageable)other : null;
        if (victim != null && CanAttack(other)) {
          victim.OnAttacked(parent.AttackPower, AttackDirection);
        }
      }
    }
  }

  public bool IsGrounded => CanStandAbove(GameManager.S.Board.GetInDirection(Row, Col, Direction.South));

  public bool WillBeAboveGroundAfterMoveInDirection(Direction direction) {
    Tile t;
    t = GameManager.S.Board.GetInDirections(Row, Col, direction, Direction.South);

    return CanStandAbove(t);
  }

  private bool CanStandAbove(Tile tile) {
    if (tile == null) {
      return true;
    }
    foreach (ITileInhabitant inhabitant in tile.Inhabitants) {
      if (inhabitant is Platform) {
        Platform platform = (Platform)inhabitant;
        return platform.IsActive;
      }
    }
    return false;
  }

  private bool CanAttack(ITileInhabitant other) {
    return other is PlayerLabel && !toIgnore.Contains(other);
  }
}

public class PlatformBeetle : Enemy<PlatformBeetle, PlatformBeetleSubEntity> {
  private readonly PlatformBeetleObject gameObject;

  private Direction _xMoveDirection = Direction.West;
  private Direction XMoveDirection {
    get => _xMoveDirection;
    set {
      if (value != Direction.West && value != Direction.East) {
        throw new System.ArgumentException("XMoveDirection must be set to eaither East or West");
      }
      _xMoveDirection = value;
    }
  }
  public PlatformAndBeetleColor GroupColor => gameObject.groupColor;

  private PlatformBeetle(PlatformBeetleObject gameObject) : base(gameObject) {
    this.gameObject = gameObject;
  }

  private int turnParity;
  public override void OnTurn() {
    //It's safe to do this stuff before OnDeath() is check in the base method.
    //Awful stylistically, but safe.  Please don't add more code unless you really know what you're doing.
    if (gameObject.moveCooldown != 0) {
      turnParity += 1;
      turnParity %= gameObject.moveCooldown;
    }
    Move();
    base.OnTurn();
  }

  public override void Destroy() {
    SoundManager.S.BeetleDied();
    PlatformToggleManager.Toggle(GroupColor);
    base.Destroy();
  }

  private class SubEntityGameObject : SingleTileEntityObject {
    public PlatformBeetle parent;
    public override float MoveAnimationTime => parent.gameObject.MoveAnimationTime;
  }

  protected override PlatformBeetleSubEntity[,] CreateSubEntities(EnemyObject e, int dim) {
    PlatformBeetleSubEntity[,] result = new PlatformBeetleSubEntity[dim, dim];
    for (int r = 0; r < dim; r++) {
      for (int c = 0; c < dim; c++) {
        SubEntityGameObject subentityGameObject = new GameObject().AddComponent<SubEntityGameObject>();
        subentityGameObject.parent = this;
        subentityGameObject.name = string.Format("PlatformBeetle[r={0}, c={1}]", r, c); ;
        subentityGameObject.spawnRow = e.spawnRow + r;
        subentityGameObject.spawnCol = e.spawnCol + c;
        subentityGameObject.transform.parent = e.transform;
        result[c, r] = new PlatformBeetleSubEntity(subentityGameObject, this);
      }
    }

    return result;
  }

  private void Move() {
    //Compute whether we are partially or entirely off the edge
    bool partiallyOffEdge = false;
    YVelocity = -1;
    foreach (PlatformBeetleSubEntity entity in Bottom()) {
      if (entity.IsGrounded) {
        YVelocity = 0;
      } else {
        partiallyOffEdge = true;
      }
    }
    if (YVelocity == -1) {
      partiallyOffEdge = false;
    }

    XVelocity = 0;
    if (YVelocity == 0 && turnParity == 0) {
      if (partiallyOffEdge) {
        bool willBeFullyOffEdgeAfterMove = true;
        foreach (PlatformBeetleSubEntity entity in Bottom()) {
          if (entity.WillBeAboveGroundAfterMoveInDirection(XMoveDirection)) {
            willBeFullyOffEdgeAfterMove = false;
            break;
          }
        }
        if (willBeFullyOffEdgeAfterMove) {
          XMoveDirection = XMoveDirection.Opposite();
        }
      } else {
        foreach (PlatformBeetleSubEntity entity in Bottom()) {
          if (!entity.WillBeAboveGroundAfterMoveInDirection(XMoveDirection)) {
            XMoveDirection = XMoveDirection.Opposite();
            break;
          }
        }
      }
      XVelocity = XMoveDirection == Direction.West ? -1 : 1;
    }
  }

  protected override void OnCollision(Direction moveDirection) {
    if (moveDirection == Direction.East) {
      XMoveDirection = Direction.West;
    }
    if (moveDirection == Direction.West) {
      XMoveDirection = Direction.East;
    }
  }

  public static PlatformBeetle Make(PlatformBeetleObject platformBeetlePrefab, int row, int col, Transform parent = null) {
    platformBeetlePrefab = Object.Instantiate(platformBeetlePrefab);
    platformBeetlePrefab.transform.parent = parent;
    platformBeetlePrefab.spawnRow = row;
    platformBeetlePrefab.spawnCol = col;
    return new PlatformBeetle(platformBeetlePrefab);
  }
}
