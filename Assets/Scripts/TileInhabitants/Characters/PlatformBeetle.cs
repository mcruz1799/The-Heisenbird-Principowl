using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeetle : Enemy
{
  private readonly PlatformBeetleObject e;

  public PlatformBeetle(SingleTileEntityObject e) : base((EnemyObject)e)
  {
    //TODO: Add Other Necessary Setup Here.
  }

  public static PlatformBeetle Make(SingleTileEntityObject platformBeetlePrefab, int row, int col, Transform parent = null)
  {
    GameObject g = platformBeetlePrefab.gameObject.transform.parent.gameObject;
    GameObject instance = Object.Instantiate(g);
    platformBeetlePrefab = instance.GetComponentInChildren<SingleTileEntityObject>();
    instance.transform.parent = parent;
    platformBeetlePrefab.spawnRow = row;
    platformBeetlePrefab.spawnCol = col;
    return new PlatformBeetle(platformBeetlePrefab);
  }
}
