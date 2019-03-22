using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableTileObject : SingleTileEntityObject
{
  public UpdraftTileMaker updraftTileMaker;
  [Range(1, 10)] public int numUpdraftTiles;
  [Range(1, 10)] public int numFireTiles;
}
