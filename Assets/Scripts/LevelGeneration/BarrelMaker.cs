using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelMaker : TileInhabitantMaker<Barrel>
{
    #pragma warning disable 0649
  [SerializeField] private BarrelObject barrelPrefab;
#pragma warning restore 0649

  public override Barrel MakeAndGet(int row, int col, Transform parent = null) {
    return Barrel.Make(barrelPrefab, row, col, parent);
  }
}
