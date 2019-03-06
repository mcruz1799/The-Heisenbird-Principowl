﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerMaker : TileInhabitantMaker {
#pragma warning disable 0649
  [SerializeField] private EnemySpawnerObject enemySpawnerPrefab;
#pragma warning restore 0649

  public override ITileInhabitant Make(int row, int col, Transform parent = null) {
    return EnemySpawner.Make(enemySpawnerPrefab, row, col, GameManager.S.TileInhabitantObjectHolder);
  }
}
