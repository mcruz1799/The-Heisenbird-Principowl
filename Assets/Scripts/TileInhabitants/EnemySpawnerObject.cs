using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerObject : SingleTileEntityObject {
  public TileInhabitantMaker enemyMaker;
  [Range(1, 20)] public int turnsBeforeRespawn = 1;
  public GameObject graphicsHolder;

  private void Awake() {
    if (enemyMaker == null) {
      Debug.LogError(gameObject.name + " doesn't have its enemyMaker initialized");
    }
  }
}
