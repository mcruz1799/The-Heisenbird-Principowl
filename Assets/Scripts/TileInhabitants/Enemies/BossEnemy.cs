using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySubEntity : EnemySubEntity<BossEnemy, BossEnemySubEntity> {
  public BossEnemySubEntity(SingleTileEntityObject gameObject, BossEnemy parent, out bool success) : base(gameObject, parent, out success) {
  }

  public override void Attack() {
    //Do nothing
  }
}

public class BossEnemy : Enemy<BossEnemy, BossEnemySubEntity> {
  private readonly BossEnemyObject gameObject;

  private BossEnemy(BossEnemyObject gameObject, out bool success) : base(gameObject, out success) {
    this.gameObject = gameObject;
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
    //Do nothing
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

