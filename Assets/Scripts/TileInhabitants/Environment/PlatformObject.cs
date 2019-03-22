using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObject : SingleTileEntityObject {

  [Header("ADJUSTABLE DURING PLAY MODE")]
  public bool isActive = true;

  //Platforms in the same group as a Beetle are toggled on/off when the Beetle is killed
  public PlatformAndBeetleColor colorGroup;

  public bool playerCanDropThrough;
  public bool playerCanJumpThrough;

#pragma warning disable 0649
  [SerializeField] private Renderer sprite;
#pragma warning restore 0649

  private void Update() {
    sprite.enabled = isActive;
  }
}
