using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableTileMaker : TileInhabitantMaker
{
#pragma warning disable 0649
  [SerializeField] private FlammableTileObject flammableTilePrefab;
#pragma warning restore 0649

  public override void Make(int row, int col, Transform parent = null) {
    FlammableTile.Make(flammableTilePrefab, row, col, parent);
  }
}
