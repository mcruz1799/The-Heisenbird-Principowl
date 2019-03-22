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
  public PlatformAndBeetleColor ColorGroup => gameObject.colorGroup;

  private Platform(SingleTileEntityObject gameObject) : base(gameObject) {
    this.gameObject = (PlatformObject)gameObject;
    PlatformToggleManager.AddPlatform(this);
  }

  public override bool CanSetPosition(int newRow, int newCol) {
    if (!base.CanSetPosition(newRow, newCol)) {
      return false;
    }

    foreach (ITileInhabitant other in GameManager.S.Board[newRow, newCol].Inhabitants) {
      if (toIgnore.Contains(other)) {
        continue;
      }

      if (other is Platform || other is PlayerLabel || other is IEnemy) {
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
    return new Platform(platformPrefab);
  }
}
