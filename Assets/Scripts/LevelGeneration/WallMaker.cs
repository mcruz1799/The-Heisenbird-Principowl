using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMaker : TileInhabitantMaker {
#pragma warning disable 0649
  [SerializeField] private PlatformObject wallPrefab;
#pragma warning restore 0649

  public override ITileInhabitant Make(int row, int col, Transform parent = null) {
    return Platform.Make(wallPrefab, row, col, parent);
  }
}
