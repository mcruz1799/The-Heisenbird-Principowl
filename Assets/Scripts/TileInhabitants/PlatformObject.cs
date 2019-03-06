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

  [SerializeField] private Renderer sprite;

  private void Update() {
    sprite.enabled = isActive;
  }
}
