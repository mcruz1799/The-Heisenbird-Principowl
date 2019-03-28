using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformToggleEnemySubEntity : EnemySubEntity<PlatformToggleEnemy, PlatformToggleEnemySubEntity> {
  private Direction _attackDirection;
  private Direction AttackDirection {
    get {
      _attackDirection = parent.XVelocity > 0 ? Direction.East : parent.XVelocity < 0 ? Direction.West : _attackDirection;
      return _attackDirection;
    }
  }

  public PlatformToggleEnemySubEntity(SingleTileEntityObject gameObject, PlatformToggleEnemy parent, out bool success) : base(gameObject, parent, out success) {
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
    return other is IPlayer && !toIgnore.Contains(other);
  }
}

public class PlatformToggleEnemy : Enemy<PlatformToggleEnemy, PlatformToggleEnemySubEntity> {
  private readonly PlatformToggleEnemyObject gameObject;

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
  public PlatformToggleGroup GroupColor => gameObject.groupColor;

  private PlatformToggleEnemy(PlatformToggleEnemyObject gameObject, out bool success) : base(gameObject, out success) {
    this.gameObject = gameObject;
  }

  private int turnParity;
  protected override void OnTurnCore() {
    //It's safe to do this stuff before OnDeath() is check in the base method.
    //Awful stylistically, but safe.  Please don't add more code unless you really know what you're doing.
    if (gameObject.moveCooldown != 0) {
      turnParity += 1;
      turnParity %= gameObject.moveCooldown;
    }
    Move();
  }

  public override void Destroy() {
    if (gameObject != null && !IsAlive) {
      SoundManager.S.BeetleDied();
      PlatformToggleManager.S.Toggle(GroupColor);
    }
    base.Destroy();
  }

  private class SubEntityGameObject : SingleTileEntityObject {
    public PlatformToggleEnemy parent;
    public override float MoveAnimationTime => parent.gameObject.MoveAnimationTime;
  }

  protected override PlatformToggleEnemySubEntity CreateSubEntity(EnemyObject e, int row, int col, out bool success) {
    SubEntityGameObject subentityGameObject = new GameObject().AddComponent<SubEntityGameObject>();
    subentityGameObject.parent = this;
    subentityGameObject.name = string.Format("PlatformBeetleEnemy[r={0}, c={1}]", row, col); ;
    subentityGameObject.spawnRow = row;
    subentityGameObject.spawnCol = col;
    subentityGameObject.transform.parent = e.transform;
    return new PlatformToggleEnemySubEntity(subentityGameObject, this, out success);
  }

  private void Move() {
    //Compute whether we are partially or entirely off the edge
    bool partiallyOffEdge = false;
    YVelocity = -1;
    foreach (PlatformToggleEnemySubEntity entity in Bottom()) {
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
        foreach (PlatformToggleEnemySubEntity entity in Bottom()) {
          if (entity.WillBeAboveGroundAfterMoveInDirection(XMoveDirection)) {
            willBeFullyOffEdgeAfterMove = false;
            break;
          }
        }
        if (willBeFullyOffEdgeAfterMove) {
          XMoveDirection = XMoveDirection.Opposite();
        }
      } else {
        foreach (PlatformToggleEnemySubEntity entity in Bottom()) {
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

  public static PlatformToggleEnemy Make(PlatformToggleEnemyObject platformBeetlePrefab, int row, int col, Transform parent = null) {
    platformBeetlePrefab = Object.Instantiate(platformBeetlePrefab);
    platformBeetlePrefab.transform.parent = parent;
    platformBeetlePrefab.spawnRow = row;
    platformBeetlePrefab.spawnCol = col;
    PlatformToggleEnemy result = new PlatformToggleEnemy(platformBeetlePrefab, out bool success);
    return success ? result : null;
  }
}
