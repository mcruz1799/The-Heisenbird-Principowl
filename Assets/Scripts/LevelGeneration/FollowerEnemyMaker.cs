using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerEnemyMaker : TileInhabitantMaker<FollowerEnemy> {
#pragma warning disable 0649
  [SerializeField] private FollowerEnemyObject followerEnemyPrefab;
#pragma warning restore 0649

  public override FollowerEnemy MakeAndGet(int row, int col, Transform parent = null) {
    return FollowerEnemy.Make(followerEnemyPrefab, row, col, parent);
  }
}
