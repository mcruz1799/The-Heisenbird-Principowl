﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerObject : SingleTileEntityObject {
  [Header("INITIALIZATION ONLY")]
#pragma warning disable 0649
  [SerializeField] private int _spawnRow;
  [SerializeField] private int _spawnCol;
#pragma warning restore 0649

  [Range(1, 1000)] public int maxHp = 1;
  [Space(10)]

  [Header("ADJUSTABLE DURING PLAY MODE")]
  public GameObject graphicsHolder;
  [Range(1, 1000)] public int attackPower = 1;

  [Header("Jumping")]
  [Range(1, 20)] public int gravity = 1;
  [Range(1, 20)] public int jumpPower = 1;

  [Range(1, 20)] public int xWallJumpPower = 1;
  [Range(1, 20)] public int yWallJumpPower = 1;
  [Range(1, 20)] public int wallSlideSpeed = 1;
  //TODO: public int wallJumpCooldown;

  [Header("Horizontal movement")]
  [Range(1, 5)] public int xAccelerationGrounded = 1;
  [Range(1, 5)] public int xAccelerationAerial = 1;
  [Range(1, 5)] public int xDeceleration = 1;

  [Header("Speed caps")]
  [Range(1, 10)] public int xSpeedMax = 1;
  [Range(1, 10)] public int ySpeedMax = 1;
  [Range(-10, -1)] public int ySpeedMin = -1;

  [Header("Skidding when turning")]
  [Range(3, 10)] public int skidAndTurnThreshold = 3;
  [Range(1, 2)] public int skidSpeed = 1; //Must be less than skidAndTurnThreshold

  private void Awake() {
    spawnRow = _spawnRow;
    spawnCol = _spawnCol;
  }
}
