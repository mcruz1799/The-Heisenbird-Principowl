﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdraftTile : SingleTileEntity {
  private readonly UpdraftTileObject updraftObject;

  public UpdraftTile(UpdraftTileObject updraftObject) : base(updraftObject) {
    this.updraftObject = updraftObject;
  }

  public override bool CanSetPosition(int newRow, int newCol) {
    if (!base.CanSetPosition(newRow, newCol)) {
      return false;
    }

    foreach (ITileInhabitant other in GameManager.S.Board[newRow, newCol].Inhabitants) {
      if (toIgnore.Contains(other)) {
        continue;
      }

      if (other is Platform) {
        return false;
      }
    }

    return true;
  }

  public static UpdraftTile Make(UpdraftTileObject updraftTilePrefab, int row, int col, Transform parent = null) {
    updraftTilePrefab = Object.Instantiate(updraftTilePrefab);
    updraftTilePrefab.transform.parent = parent;
    updraftTilePrefab.spawnRow = row;
    updraftTilePrefab.spawnCol = col;
    return new UpdraftTile(updraftTilePrefab);
  }
}
