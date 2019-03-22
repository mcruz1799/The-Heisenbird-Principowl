using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerEnemyMaker : EnemyMaker {
#pragma warning disable 0649
  [SerializeField] private FollowerEnemyObject followerEnemyPrefab;
#pragma warning restore 0649

  private IEnemy mostRecentlyMade;
  public override IEnemy MostRecentlyMade => mostRecentlyMade;

  public override void Make(int row, int col, Transform parent = null) {
    mostRecentlyMade = FollowerEnemy.Make(followerEnemyPrefab, row, col, parent);
  }
}
