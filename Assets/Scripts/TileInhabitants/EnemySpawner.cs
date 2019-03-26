using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : SingleTileEntity, ITurnTaker {
  private readonly EnemySpawnerObject gameObject;
  private IEnemy enemy;

  private EnemySpawner(EnemySpawnerObject gameObject, out bool success) : base(gameObject, out success) {
    if (success) {
      GameManager.S.RegisterTurnTaker(this);
      this.gameObject = gameObject;
    }
  }

  public static EnemySpawner Make(EnemySpawnerObject enemySpawnerPrefab, int row, int col, Transform parent = null) {
    enemySpawnerPrefab = Object.Instantiate(enemySpawnerPrefab);
    enemySpawnerPrefab.transform.parent = parent;
    enemySpawnerPrefab.spawnRow = row;
    enemySpawnerPrefab.spawnCol = col;
    EnemySpawner result = new EnemySpawner(enemySpawnerPrefab, out bool success);
    return success ? result : null;
  }

  private int turnsUntilRespawn;
  public void OnTurn() {
    if (enemy != null && enemy.IsAlive) {
      return;
    }

    turnsUntilRespawn -= 1;

    if (turnsUntilRespawn <= 0) {
      turnsUntilRespawn = gameObject.turnsBeforeRespawn;
      object makeResult = gameObject.enemyMaker.Make(Row, Col, gameObject.transform);
      if (makeResult is IEnemy) {
        enemy = (IEnemy)makeResult;
      }
    }
  }
}
