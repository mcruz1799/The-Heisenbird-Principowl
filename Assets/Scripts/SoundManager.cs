using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  public static SoundManager S { get; private set; }

  private void Awake() {
    S = this;
  }

  public void PlayerHeadBonk() {

  }

  public void PlayerLanded() {

  }

  public void PlayerJump() {

  }

  public void PlayerDamaged() {

  }

  public void PlayerDied() {

  }
}
