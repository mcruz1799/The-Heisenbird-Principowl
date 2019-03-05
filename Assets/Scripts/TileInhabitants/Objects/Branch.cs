using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MultiTileEntity {
  private Branch(HashSet<SingleTileEntity> composedEntities, SingleTileEntity leadingEntity) : base(composedEntities, leadingEntity) { }

  public static Branch Make(PlatformObject platformPrefab, int topLeftRow, int topLeftCol, int width, Transform parent = null) {
    HashSet<SingleTileEntity> composedEntities = new HashSet<SingleTileEntity>();
    SingleTileEntity leadingEntity = null;
    for (int i = 0; i < width; i++) {
      Platform platform = Platform.Make(platformPrefab, topLeftRow, topLeftCol + i, parent);
      if (leadingEntity == null) leadingEntity = platform;
      composedEntities.Add(platform);
    }

    if (leadingEntity == null) {
      throw new System.Exception("Failed to create Branch");
    }
    return new Branch(composedEntities, leadingEntity);
  }
}
