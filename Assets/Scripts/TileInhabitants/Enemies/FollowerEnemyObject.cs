using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerEnemyObject : EnemyObject {
  [Range(1, 300)] public int aggroRange = 100;
  [Range(1, 300)] public int yAggroRange = 1;
}
