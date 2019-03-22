using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private SpriteAnimator idleGrounded;
  [SerializeField] private SpriteAnimator jumping;
  [SerializeField] private SpriteAnimator running;
  private SpriteAnimator current_animator = null;

  //[SerializeField] private ISpriteAnimator midair;
#pragma warning restore 0649

  private Player Player => GameManager.S.Player;

  private void Update() {
    //Flip PlayerGraphic Orientation
    if (Player.YVelocity != 0) {
      if(jumping.IsPaused){
        disableAllAnimators();
        jumping.IsVisible = true;
        jumping.IsPaused = false;
        jumping.StartFromFirstFrame();
        current_animator = jumping;
      }
      this.flipPlayerX(jumping);
    }
    else if (Player.XVelocity != 0){
      if(running.IsPaused){
        disableAllAnimators();
        running.IsVisible = true;
        running.IsPaused = false;
        running.StartFromFirstFrame();
        current_animator = running;
      }
      this.flipPlayerX(running);
    }
    else{
      if(idleGrounded.IsPaused){
        disableAllAnimators();
        idleGrounded.IsVisible = true;
        idleGrounded.IsPaused = false;
        idleGrounded.StartFromFirstFrame();
        current_animator = idleGrounded;
      }
      this.flipPlayerX(idleGrounded);
    }
    if(current_animator) current_animator.FlipY = Player.IsStunned;
  }

  private void disableAllAnimators(){
    idleGrounded.IsPaused = true;
    idleGrounded.IsVisible = false;
    jumping.IsPaused = true;
    jumping.IsVisible = false;
    running.IsPaused = true;
    running.IsVisible = false;
  }

  private void flipPlayerX(SpriteAnimator s){
    s.FlipX = Player.XVelocity >= 0 ? false : true;
  }
}
