using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdraftTileMaker : TileInhabitantMaker
{
#pragma warning disable 0649
  [SerializeField] private UpdraftTileObject updraftTilePrefab;
#pragma warning restore 0649

  public override void Make(int row, int col, Transform parent = null) {
    UpdraftTile.Make(updraftTilePrefab, row, col, parent);
  }
}
