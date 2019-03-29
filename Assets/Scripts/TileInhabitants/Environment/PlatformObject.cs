using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformObject : SingleTileEntityObject {

  [Header("ADJUSTABLE DURING PLAY MODE")]
  public bool isActive = true;

  //Platforms in the same group as a Beetle are toggled on/off when the Beetle is killed
  public PlatformToggleGroup colorGroup;

  public bool playerCanDropThrough;
  public bool playerCanJumpThrough;

#pragma warning disable 0649
  [SerializeField] private Renderer graphic;
  [SerializeField] private Material activeMaterial;
  [SerializeField] private Material inactiveMaterial;
#pragma warning restore 0649

  private void Awake() {
    if (graphic == null) {
      graphic = GetComponent<Renderer>();
    }
  }

  private void Update() {
    if (isActive) {
      if (activeMaterial != null) {
        graphic.enabled = true;
        graphic.material = activeMaterial;
      } else {
        throw new System.Exception("Uninitialized active material");
      }
    } else {
      if (inactiveMaterial != null) {
        graphic.enabled = true;
        graphic.material = inactiveMaterial;
      } else {
        graphic.enabled = false;
      }
    }
  }
}
