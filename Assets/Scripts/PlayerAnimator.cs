using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private SpriteAnimator idleGrounded;
  //[SerializeField] private ISpriteAnimator midair;
#pragma warning restore 0649

  private Player Player => GameManager.S.Player;

  private void Update() {
    //Flip PlayerGraphic Orientation
    if (Player.XVelocity < 0) {
      idleGrounded.FlipX = true;
      //midair.FlipX = true;
    } else if (Player.XVelocity > 0) {
      idleGrounded.FlipX = false;
      //midair.FlipX = false;
    }

    idleGrounded.FlipY = Player.IsStunned;
  }
}
