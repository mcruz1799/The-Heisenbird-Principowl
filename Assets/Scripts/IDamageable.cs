using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {
  int Hitpoints { get; }
  int MaxHitpoints { get; }
  bool IsAlive { get; }
  int CalculateDamage(int baseDamage);
  void TakeDamage(int baseDamage);
}