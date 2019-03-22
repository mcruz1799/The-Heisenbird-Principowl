using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdraftTileObject : SingleTileEntityObject
{
  public bool isActive = false;
#pragma warning disable 0649
  [SerializeField] private Renderer sprite;
#pragma warning restore 0649

  private void Update() {
    sprite.enabled = isActive;
  }

}
