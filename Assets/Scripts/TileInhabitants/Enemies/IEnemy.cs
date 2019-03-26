using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy {
  bool IsAlive { get; }
  int XVelocity { get; }
  int YVelocity { get; }
}