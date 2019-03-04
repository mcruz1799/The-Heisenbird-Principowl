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

  public int CalculateDamage(int baseDamage) {
    return baseDamage;
  }

  public void TakeDamage(int baseDamage) {
    Hitpoints -= CalculateDamage(baseDamage);
    if (Hitpoints < 0) {
      Hitpoints = 0;
    }
  }
}
