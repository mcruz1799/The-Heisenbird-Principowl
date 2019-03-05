using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : SingleTileEntity {
  private readonly PlatformObject gameObject;

  public bool IsActive => gameObject.isActive;
  public bool PlayerCanDropThrough => gameObject.playerCanDropThrough;
  public bool PlayerCanJumpThrough => gameObject.playerCanJumpThrough;
  public PlatformAndBeetleColor ColorGroup => gameObject.colorGroup;

  private Platform(SingleTileEntityObject gameObject) : base(gameObject) {
    this.gameObject = (PlatformObject)gameObject;
  }

  protected override bool IsBlockedByCore(ITileInhabitant other) {
    return other is Platform || other is Player || other is Enemy;
  }

  public static Platform Make(PlatformObject platformPrefab, int row, int col, Transform parent = null) {
    platformPrefab = Object.Instantiate(platformPrefab);
    platformPrefab.spawnRow = row;
    platformPrefab.spawnCol = col;
    platformPrefab.transform.parent = parent;
    return new Platform(platformPrefab);
  }
}
