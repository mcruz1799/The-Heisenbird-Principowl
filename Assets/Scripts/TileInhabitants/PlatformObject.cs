using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObject : SingleTileEntityObject {
  public bool isActive = true;

  [Header("ADJUSTABLE DURING PLAY MODE")]

  //Platforms in the same group as a Beetle are toggled on/off when the Beetle is killed
  public PlatformAndBeetleColor colorGroup;

  public bool playerCanDropThrough;
  public bool playerCanJumpThrough;

}
