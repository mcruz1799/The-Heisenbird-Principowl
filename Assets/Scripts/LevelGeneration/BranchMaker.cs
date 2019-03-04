using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchMaker : TileInhabitantMaker {
#pragma warning disable 0649
  [Range(1, 5)] [SerializeField] private int platformLength = 1;
  [SerializeField] private SingleTileEntityObject subentityObjectPrefab;
#pragma warning restore 0649

  public override ITileInhabitant Make(int topLeftRow, int topLeftCol, Transform parent = null) {
    GameObject subentityHolder = new GameObject();
    subentityHolder.transform.parent = parent;
    subentityHolder.name = subentityObjectPrefab.name;
    return Branch.Make(subentityObjectPrefab, topLeftRow, topLeftCol, platformLength, subentityHolder.transform);
  }
}
