using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileInhabitant {
  //Returns a set containing the Tiles occupied by this
  ISet<Tile> Occupies();

  //Returns whether this can occupy the same Tile as other
  bool IsBlockedBy(ITileInhabitant other);
}
