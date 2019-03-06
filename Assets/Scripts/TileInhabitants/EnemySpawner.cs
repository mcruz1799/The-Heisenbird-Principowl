using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : SingleTileEntity, ITurnTaker {
  private readonly EnemySpawnerObject gameObject;
  private Enemy enemy;

  private EnemySpawner(SingleTileEntityObject gameObject) : base(gameObject) {
    GameManager.S.RegisterTurnTaker(this);
    this.gameObject = (EnemySpawnerObject)gameObject;
  }

  protected override bool IsBlockedByCore(ITileInhabitant other) {
    return false;
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
        if (inhabitant is Enemy || inhabitant is Platform || inhabitant is Player) {
          return;
        }
      }
      enemy = (Enemy)gameObject.enemyMaker.Make(Row, Col, gameObject.transform);
    }
  }
}
