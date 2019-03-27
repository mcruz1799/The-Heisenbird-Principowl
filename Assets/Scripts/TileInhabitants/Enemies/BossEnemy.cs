using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySubEntity : EnemySubEntity<BossEnemy, BossEnemySubEntity> {
  private Direction AttackDirection => Direction.South;

  public BossEnemySubEntity(SingleTileEntityObject gameObject, BossEnemy parent, out bool success) : base(gameObject, parent, out success) {
  }

  public override void Attack() {}

  private bool CanAttack(ITileInhabitant other) {
    return other is IPlayer && !toIgnore.Contains(other);
  }
}

public class BossEnemy : Enemy<BossEnemy, BossEnemySubEntity> {
  private readonly BossEnemyObject gameObject;
  private readonly BarrelMaker barrelMaker;
  private readonly int bossHeight;

  private BossEnemy(BossEnemyObject gameObject, out bool success) : base(gameObject, out success) {
    this.gameObject = gameObject;
    this.barrelMaker = gameObject.barrelMaker;
    this.bossHeight = gameObject.bossHeight;
  }


  public override void Destroy() {
    //TODO: replace with boss bonk sound, play some destroy animation
    SoundManager.S.BeetleDied();
    base.Destroy();
  }

  protected override void OnCollision(Direction moveDirection) {
    //Do nothing
  }

  protected override void OnTurnCore(){
    //instantiates barrel every couple of random seconds
    int r = Random.Range(0, 11); //10% chance to throw a barrel
    if (r == 5) barrelMaker.MakeAndGet(TopLeft.Row - bossHeight, TopLeft.Col + 2, null);
  }

  public override void OnAttacked(int attackPower, Direction attackDirection){
    GameManager.S.LoadNextLevel();
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

  public static BossEnemy Make(BossEnemyObject bossEnemyPrefab, int row, int col, Transform parent = null) {
    bossEnemyPrefab = Object.Instantiate(bossEnemyPrefab);
    bossEnemyPrefab.transform.parent = parent;
    bossEnemyPrefab.spawnRow = row;
    bossEnemyPrefab.spawnCol = col;
    BossEnemy result = new BossEnemy(bossEnemyPrefab, out bool success);
    return success ? result : null;
  }
}

