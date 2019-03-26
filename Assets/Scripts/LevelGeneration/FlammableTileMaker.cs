using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableTileMaker : TileInhabitantMaker<FlammableTile> {
#pragma warning disable 0649
  [SerializeField] private FlammableTileObject flammableTilePrefab;
#pragma warning restore 0649

  public override FlammableTile MakeAndGet(int row, int col, Transform parent = null) {
    return FlammableTile.Make(flammableTilePrefab, row, col, parent);
  }
}
