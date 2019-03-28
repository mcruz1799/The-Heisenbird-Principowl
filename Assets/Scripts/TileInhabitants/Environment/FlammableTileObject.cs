using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableTileObject : SingleTileEntityObject {
  public UpdraftTileMaker updraftTileMaker;
  [Range(1, 50)] public int numUpdraftTiles = 1;

#pragma warning disable 0649
  [SerializeField] private GameObject onFireGraphic;
  [SerializeField] private GameObject notOnFireGraphic;
#pragma warning restore 0649

  public void UpdateGraphic(bool isOnFire) {
    onFireGraphic.SetActive(isOnFire);
    notOnFireGraphic.SetActive(!isOnFire);
  }
}
