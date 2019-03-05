using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeetleMaker : TileInhabitantMaker
{
#pragma warning disable 0649
  [SerializeField] private GameObject platformBeetlePrefab;
#pragma warning restore 0649

  public override ITileInhabitant Make(int row, int col, Transform parent = null)
  {
    Debug.Log("Shizzwizzle.");
    SingleTileEntityObject obj = (SingleTileEntityObject) platformBeetlePrefab.GetComponentInChildren<PlatformBeetleObject>();
    Debug.Log(obj);
    return PlatformBeetle.Make(obj, row, col, parent);
  }
}

