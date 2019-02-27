using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMaker : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private Vector2Int playerSpawn;
  [SerializeField] private GameObject wallPrefab;
#pragma warning restore 0649

  public ISet<ITileInhabitant> PopulateTile(int row, int col) {
    return PopulateTile1(row, col);
  }

  private ISet<ITileInhabitant> PopulateTile1(int row, int col) {
    ISet<ITileInhabitant> result = new HashSet<ITileInhabitant>();

    if (row == 0) {
      Wall wall = Instantiate(wallPrefab.GetComponentInChildren<Wall>());
      result.Add(wall);
    }

    if ((row == playerSpawn.y) && (col == playerSpawn.x)) {
      GameManager.S.Player.SetPosition(row, col, out bool success);
      if (!success) {
        throw new System.Exception("Failed to place player");
      }
    }

    return result;
  }
}
