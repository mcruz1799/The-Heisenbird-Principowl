﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeetleObject : EnemyObject {
  public PlatformAndBeetleColor groupColor;
  [Range(0, 10)] public int moveCooldown;
}
