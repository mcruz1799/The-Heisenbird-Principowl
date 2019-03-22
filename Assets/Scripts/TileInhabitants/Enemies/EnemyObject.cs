using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyObject : SingleTileEntityObject {
#pragma warning disable 0649
  [Range(1, 1000)] public int maxHp = 1;
  [Range(1, 5)] public int dim = 1;


  [Header("ADJUSTABLE DURING PLAY MODE")]

  [Range(1, 1000)] public int attackStunTurns = 1;

  [Header("Speed caps")]
  [Range(1, 10)] public int xSpeedMax = 1;
  [Range(1, 10)] public int ySpeedMax = 1;
  [Range(-10, -1)] public int ySpeedMin = -1;

  public GameObject graphicsHolder;
#pragma warning restore 0649
}
