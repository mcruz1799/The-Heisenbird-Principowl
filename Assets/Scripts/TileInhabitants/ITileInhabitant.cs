using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileInhabitant {
  void SetPosition(int newRow, int newCol);
  bool CanSetPosition(int newRow, int newCol);

  //Returns a set containing the Tiles occupied by this
  ISet<Tile> Occupies();
}
