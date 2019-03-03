using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker {
  bool CanAttack(IDamageable other);
  void Attack(IDamageable other);
}
