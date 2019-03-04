using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : SingleTileEntity {
  
  //A Wall cannot be placed in a Tile that has any other occupant
  public override bool IsBlockedBy(ITileInhabitant other) {
    return true;
  }
}
