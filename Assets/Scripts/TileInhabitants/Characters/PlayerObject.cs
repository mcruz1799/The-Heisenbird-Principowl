using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerObject : SingleTileEntityObject {
  [Range(1, 1000)] public int _maxHp = 1;
  [Space(10)]

  [Header("ADJUSTABLE DURING PLAY MODE")]
  [Range(1, 1000)] public int _attackPower = 1;

  [Header("Jumping")]
  [Range(1, 20)] public int gravity = 1;
  [Range(1, 20)] public int jumpPower = 1;

  [Range(1, 20)] public int _xWallJumpPower = 1;
  [Range(1, 20)] public int yWallJumpPower = 1;
  [Range(1, 20)] public int wallSlideSpeed = 1;
  //TODO: public int wallJumpCooldown;

  [Header("Horizontal movement")]
  [Range(1, 5)] public int _xAccelerationGrounded = 1;
  [Range(1, 5)] public int _xAccelerationAerial = 1;
  [Range(1, 5)] public int xDeceleration = 1;

  [Header("Speed caps")]
  [Range(1, 10)] public int xSpeedMax = 1;
  [Range(1, 10)] public int ySpeedMax = 1;
  [Range(-10, -1)] public int ySpeedMin = -1;

  [Header("Skidding when turning")]
  [Range(3, 10)] public int skidAndTurnThreshold = 3;
  [Range(1, 2)] public int skidSpeed = 1; //Must be less than skidAndTurnThreshold
}
