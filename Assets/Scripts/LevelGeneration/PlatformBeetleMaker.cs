using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeetleMaker : TileInhabitantMaker<PlatformBeetle> {
#pragma warning disable 0649
  [SerializeField] private PlatformBeetleObject platformBeetlePrefab;
#pragma warning restore 0649

  public override PlatformBeetle MakeAndGet(int row, int col, Transform parent = null) {
    return PlatformBeetle.Make(platformBeetlePrefab, row, col, parent);
  }
}

