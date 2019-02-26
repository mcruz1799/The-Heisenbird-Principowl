using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A Tile is a container for ITileInhabitants
public sealed class Tile : MonoBehaviour {
  private ISet<ITileInhabitant> inhabitants;

  private void Awake() {
    inhabitants = new HashSet<ITileInhabitant>();
  }

  //Check whether an inhabitant can be added to this Tile
  public bool CanAdd(ITileInhabitant newInhabitant) {
    bool alreadyAdded = inhabitants.Contains(newInhabitant);

    //As an example, a Player cannot be added to a Tile that contains a Wall
    bool conflictsWithInhabitant = false;
    foreach (ITileInhabitant other in inhabitants) {
      if (newInhabitant.IsBlockedBy(other)) {
        conflictsWithInhabitant = true;
        break;
      }
    }

    return !alreadyAdded && !conflictsWithInhabitant;
  }

  public void Add(ITileInhabitant newInhabitant) {
    if (!CanAdd(newInhabitant)) {
      Debug.LogError("Illegal call to Tile.Add()");
      throw new System.ArgumentException("Cannot add inhabitant");
    }

    inhabitants.Add(newInhabitant);
  }

  public bool Remove(ITileInhabitant inhabitant) {
    return inhabitants.Remove(inhabitant);
  }
}