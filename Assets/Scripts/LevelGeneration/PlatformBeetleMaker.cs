using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeetleMaker : EnemyMaker {
#pragma warning disable 0649
  [SerializeField] private PlatformBeetleObject platformBeetlePrefab;
#pragma warning restore 0649

  private IEnemy mostRecentlyMade;
  public override IEnemy MostRecentlyMade => mostRecentlyMade;

  public override void Make(int row, int col, Transform parent = null) {
    mostRecentlyMade = PlatformBeetle.Make(platformBeetlePrefab, row, col, parent);
  }
}

