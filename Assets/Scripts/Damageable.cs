using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable {
  public int Hitpoints { get; private set; }
  public int MaxHitpoints { get; private set; }
  public bool IsAlive => Hitpoints > 0;

  public Damageable(int maxHp) {
    MaxHitpoints = maxHp;
    Hitpoints = MaxHitpoints;
  }

  public void TakeDamage(int baseDamage) {
    if (baseDamage > Hitpoints) {
      Hitpoints = 0;
    } else {
      Hitpoints -= baseDamage;
    }
  }
}
