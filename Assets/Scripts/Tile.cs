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

  [System.Obsolete("Please loop over Tile.Inhabitants and attack them by trying to cast them to IDamageable")]
  public void Attack(IAttacker attacker) {
    Debug.LogWarning("Tile.Attack() is deprecated");
    foreach (ITileInhabitant inhabitant in inhabitants) {
      IDamageable victim = inhabitant is IDamageable ? (IDamageable)inhabitant : null;
      if (victim != null && attacker.CanAttack(victim)) {
        attacker.Attack(victim);
      }
    }
  }
}