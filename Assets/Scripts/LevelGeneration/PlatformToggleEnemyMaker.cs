using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformToggleEnemyMaker : TileInhabitantMaker<PlatformToggleEnemy> {
#pragma warning disable 0649
  [SerializeField] private PlatformToggleEnemyObject platformBeetlePrefab;
#pragma warning restore 0649

  public override PlatformToggleEnemy MakeAndGet(int row, int col, Transform parent = null) {
    return PlatformToggleEnemy.Make(platformBeetlePrefab, row, col, parent);
  }
}

