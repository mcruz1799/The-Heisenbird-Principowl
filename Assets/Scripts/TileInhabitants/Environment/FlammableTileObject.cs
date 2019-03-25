using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableTileObject : SingleTileEntityObject {
  public UpdraftTileMaker updraftTileMaker;
  [Range(1, 50)] public int numUpdraftTiles = 1;
}
