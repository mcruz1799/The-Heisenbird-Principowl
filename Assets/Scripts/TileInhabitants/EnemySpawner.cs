using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : SingleTileEntity, ITurnTaker {
  private readonly EnemySpawnerObject gameObject;
  private IEnemy enemy;

  private EnemySpawner(EnemySpawnerObject gameObject) : base(gameObject) {
    GameManager.S.RegisterTurnTaker(this);
    this.gameObject = gameObject;
  }

  public static EnemySpawner Make(EnemySpawnerObject enemySpawnerPrefab, int row, int col, Transform parent = null) {
    enemySpawnerPrefab = Object.Instantiate(enemySpawnerPrefab);
    enemySpawnerPrefab.transform.parent = parent;
    enemySpawnerPrefab.spawnRow = row;
    enemySpawnerPrefab.spawnCol = col;
    return new EnemySpawner(enemySpawnerPrefab);
  }

  private int turnsUntilRespawn;
  public void OnTurn() {
    if (enemy != null && enemy.IsAlive) {
      return;
    }

    turnsUntilRespawn -= 1;

    if (turnsUntilRespawn <= 0) {
      turnsUntilRespawn = gameObject.turnsBeforeRespawn;
      foreach (ITileInhabitant inhabitant in GameManager.S.Board[Row, Col].Inhabitants) {
        if (inhabitant is IEnemy || inhabitant is Platform || inhabitant is PlayerLabel) {
          return;
        }
      }
      try {
        gameObject.enemyMaker.Make(Row, Col, gameObject.transform);
      } catch {
        Debug.LogError("Failed attempt to create an enemy");
        return;
      }
      enemy = gameObject.enemyMaker.MostRecentlyMade;
    }
  }
}
