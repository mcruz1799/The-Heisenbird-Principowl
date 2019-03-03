using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
  int Hitpoints { get; }
  int MaxHitpoints { get; }
  bool IsAlive { get;
  }
  int CalculateDamage(IAttacker attacker, int baseDamage);
  void TakeDamage(IAttacker attacker, int baseDamage);
}