using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
  int CalculateDamage(IAttacker attacker, int baseDamage);
  void TakeDamage(IAttacker attacker, int baseDamage);
}