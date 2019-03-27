using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdraftTile : SingleTileEntity {
  private readonly UpdraftTileObject gameObject;

  public UpdraftTile(UpdraftTileObject updraftObject, out bool success) : base(updraftObject, out success) {
    if (success) {
      this.gameObject = updraftObject;
    }
  }

  public override bool CanSetPosition(int newRow, int newCol) {
    if (!base.CanSetPosition(newRow, newCol)) {
      return false;
    }

    foreach (ITileInhabitant other in GameManager.S.Board[newRow, newCol].Inhabitants) {
      if (toIgnore.Contains(other)) {
        continue;
      }

      if (other is Platform || other is FlammableTile) {
        return false;
      }
    }

    return true;
  }

  public void SetGraphicVisiblity(bool isVisible) {
    gameObject.updraftGraphic.enabled = isVisible;
  }

  public static UpdraftTile Make(UpdraftTileObject updraftTilePrefab, int row, int col, Transform parent = null) {
    updraftTilePrefab = Object.Instantiate(updraftTilePrefab);
    updraftTilePrefab.transform.parent = parent;
    updraftTilePrefab.spawnRow = row;
    updraftTilePrefab.spawnCol = col;
    UpdraftTile result = new UpdraftTile(updraftTilePrefab, out bool success);
    return success ? result : null;
  }
}
