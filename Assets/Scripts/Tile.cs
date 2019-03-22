using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A Tile is a container for ITileInhabitants
public sealed class Tile : MonoBehaviour {
  private readonly ISet<ITileObserver> observers = new HashSet<ITileObserver>();
  private readonly HashSet<ITileInhabitant> inhabitants = new HashSet<ITileInhabitant>();
  public IReadOnlyCollection<ITileInhabitant> Inhabitants => inhabitants;

  public bool Subscribe(ITileObserver observer) {
    return observers.Add(observer);
  }
  public bool Unsubscribe(ITileObserver observer) {
    return observers.Remove(observer);
  }

  public void Add(ITileInhabitant newInhabitant) {
    inhabitants.Add(newInhabitant);
    foreach (ITileObserver observer in observers) {
      observer.OnInhabitantEntered(newInhabitant);
    }
  }

  public bool Remove(ITileInhabitant inhabitant) {
    bool success = inhabitants.Remove(inhabitant);
    if (success) {
      foreach (ITileObserver observer in observers) {
        observer.OnInhabitantExited(inhabitant);
      }
    }

    return success;
  }
}