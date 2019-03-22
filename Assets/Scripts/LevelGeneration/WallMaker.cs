using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMaker : TileInhabitantMaker {
#pragma warning disable 0649
  [SerializeField] private PlatformObject wallPrefab;
#pragma warning restore 0649

  public override void Make(int row, int col, Transform parent = null) {
    Platform.Make(wallPrefab, row, col, parent);
  }
}
