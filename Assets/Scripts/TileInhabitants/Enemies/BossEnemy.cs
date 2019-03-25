using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySubEntity : EnemySubEntity<BossEnemy, BossEnemySubEntity> {
  private Direction AttackDirection => Direction.South;

  public BossEnemySubEntity(SingleTileEntityObject gameObject, BossEnemy parent) : base(gameObject, parent) {
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

  private BossEnemy(BossEnemyObject gameObject) : base(gameObject) {
    this.gameObject = gameObject;
  }


  public override void Destroy() {
    //TODO: replace with boss bonk sound
    SoundManager.S.BeetleDied();
    base.Destroy();
  }

  //TODO: modify onturn to throw barrels sometimes?
  public override void OnAttacked(int attackPower, Direction attackDirection){
    //TODO: check what current level is, load new level or end game
  }

  private class SubEntityGameObject : SingleTileEntityObject {
    public BossEnemy parent;
    public override float MoveAnimationTime => parent.gameObject.MoveAnimationTime;
  }

  protected override BossEnemySubEntity[,] CreateSubEntities(EnemyObject e, int dim) {
    BossEnemySubEntity[,] result = new BossEnemySubEntity[dim, dim];
    for (int r = 0; r < dim; r++) {
      for (int c = 0; c < dim; c++) {
        SubEntityGameObject subentityGameObject = new GameObject().AddComponent<SubEntityGameObject>();
        subentityGameObject.parent = this;
        subentityGameObject.name = string.Format("BossEnemy[r={0}, c={1}]", r, c); ;
        subentityGameObject.spawnRow = e.spawnRow + r;
        subentityGameObject.spawnCol = e.spawnCol + c;
        subentityGameObject.transform.parent = e.transform;
        result[c, r] = new BossEnemySubEntity(subentityGameObject, this);
      }
    }

    return result;
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
    return new BossEnemy(bossEnemyPrefab);
  }
}

