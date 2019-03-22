using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileInhabitant {
  void SetPosition(int newRow, int newCol, out bool success);

  //Returns a set containing the Tiles occupied by this
  ISet<Tile> Occupies();
}
