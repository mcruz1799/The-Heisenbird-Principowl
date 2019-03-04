using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyObject : SingleTileEntityObject {
#pragma warning disable 0649
  [Range(1, 1000)] public int _maxHp = 1;
  [Range(1, 1000)] public int _attackPower = 1;
#pragma warning restore 0649
}
