using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySubEntity : EnemySubEntity<BossEnemy, BossEnemySubEntity> {
  private Direction AttackDirection => Direction.South;

  public BossEnemySubEntity(SingleTileEntityObject gameObject, BossEnemy parent, out bool success) : base(gameObject, parent, out success) {
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

public class BossEnemy : Enemy<BossEnemy, BossEnemySubEntity> {
  private readonly BossEnemyObject gameObject;

  private BossEnemy(BossEnemyObject gameObject, out bool success) : base(gameObject, out success) {
    this.gameObject = gameObject;
  }


  public override void Destroy() {
    //TODO: replace with boss bonk sound
    SoundManager.S.BeetleDied();
    base.Destroy();
  }

  protected override void OnCollision(Direction moveDirection) {
    //TODO:
    return;
  }

  //TODO: modify onturn to throw barrels sometimes?
  public override void OnAttacked(int attackPower, Direction attackDirection){
    //TODO: check what current level is, load new level or end game
  }

  private class SubEntityGameObject : SingleTileEntityObject {
    public BossEnemy parent;
    public override float MoveAnimationTime => parent.gameObject.MoveAnimationTime;
  }

  protected override BossEnemySubEntity CreateSubEntity(EnemyObject e, int row, int col, out bool success) {
    SubEntityGameObject subentityGameObject = new GameObject().AddComponent<SubEntityGameObject>();
    subentityGameObject.parent = this;
    subentityGameObject.name = string.Format("BossEnemy[r={0}, c={1}]", row, col); ;
    subentityGameObject.spawnRow = row;
    subentityGameObject.spawnCol = col;
    subentityGameObject.transform.parent = e.transform;
    return new BossEnemySubEntity(subentityGameObject, this, out success);
  }

  /*protected override void OnCollision(Direction moveDirection) {
    if (moveDirection == Direction.East) {
      XMoveDirection = Direction.West;
    }
    if (moveDirection == Direction.West) {
      XMoveDirection = Direction.East;
    }
  }*/

  public static BossEnemy Make(BossEnemyObject bossEnemyPrefab, int row, int col, Transform parent = null) {
    bossEnemyPrefab = Object.Instantiate(bossEnemyPrefab);
    bossEnemyPrefab.transform.parent = parent;
    bossEnemyPrefab.spawnRow = row;
    bossEnemyPrefab.spawnCol = col;
    BossEnemy result = new BossEnemy(bossEnemyPrefab, out bool success);
    return success ? result : null;
  }
}

