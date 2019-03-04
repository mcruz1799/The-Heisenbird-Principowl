using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MultiTileEntity
{
  private Branch(HashSet<SingleTileEntity> composedEntities, SingleTileEntity leadingEntity) : base(composedEntities, leadingEntity) { }

  public static Branch Make(SingleTileEntityObject subentityObjectPrefab, int topLeftRow, int topLeftCol, int width, Transform parent = null) {
    HashSet<SingleTileEntity> composedEntities = new HashSet<SingleTileEntity>();
    SingleTileEntity leadingEntity = null;
    for (int i = 0; i < width; i++) {
      Wall wall = Wall.Make(subentityObjectPrefab, topLeftRow, topLeftCol + i, parent);
      if (leadingEntity == null) leadingEntity = wall;
      composedEntities.Add(wall);
    }

    if (leadingEntity == null) {
      throw new System.Exception("Failed to create Branch");
    }
    return new Branch(composedEntities, leadingEntity);
  }
}
