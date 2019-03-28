using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : SingleTileEntity {
  private readonly PlatformObject gameObject;

  public bool IsActive {
    get => gameObject.isActive;
    set => gameObject.isActive = value;
  }
  public bool PlayerCanDropThrough => gameObject.playerCanDropThrough;
  public bool PlayerCanJumpThrough => gameObject.playerCanJumpThrough;
  public PlatformToggleGroup ColorGroup => gameObject.colorGroup;

  private Platform(SingleTileEntityObject gameObject, out bool success) : base(gameObject, out success) {
    if (success) {
      this.gameObject = (PlatformObject)gameObject;
      PlatformToggleManager.S.AddPlatform(this);
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

      if (other is Platform) {
        return false;
      }
    }

    return true;
  }

  public static Platform Make(PlatformObject platformPrefab, int row, int col, Transform parent = null) {
    platformPrefab = Object.Instantiate(platformPrefab);
    platformPrefab.spawnRow = row;
    platformPrefab.spawnCol = col;
    platformPrefab.transform.parent = parent;
    Platform result = new Platform(platformPrefab, out bool success);
    return success ? result : null;
  }
}
