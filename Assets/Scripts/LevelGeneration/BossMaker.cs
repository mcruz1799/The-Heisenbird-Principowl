using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMaker : TileInhabitantMaker<BossEnemy>
{
    #pragma warning disable 0649
  [SerializeField] private BossEnemyObject bossEnemyPrefab;
#pragma warning restore 0649

  public override BossEnemy MakeAndGet(int row, int col, Transform parent = null) {
    return BossEnemy.Make(bossEnemyPrefab, row, col, parent);
  }
}
