using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : SingleTileEntity {

  private Wall(SingleTileEntityObject wallObject) : base(wallObject) { }
  
  //A Wall cannot be placed in a Tile that has any other occupant
  public override bool IsBlockedBy(ITileInhabitant other) {
    return true;
  }

  public static Wall Make(SingleTileEntityObject wallPrefab, int row, int col, Transform parent = null) {
    wallPrefab = Object.Instantiate(wallPrefab);
    wallPrefab.transform.parent = parent;
    wallPrefab.spawnRow = row;
    wallPrefab.spawnCol = col;
    return new Wall(wallPrefab);
  }
}
