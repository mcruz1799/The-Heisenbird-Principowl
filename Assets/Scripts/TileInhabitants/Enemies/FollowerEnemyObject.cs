using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerEnemyObject : EnemyObject
{
  [Range(1, 10)] public int aggroRange;
  public int maxDistFromHome;
}
