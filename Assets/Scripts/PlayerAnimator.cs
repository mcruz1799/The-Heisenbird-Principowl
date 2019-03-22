using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private SpriteAnimator idleGrounded;
  [SerializeField] private SpriteAnimator midair;
  [SerializeField] private SpriteAnimator running;
#pragma warning restore 0649

  private ISpriteAnimator current = null;
  private readonly List<ISpriteAnimator> animators = new List<ISpriteAnimator>();

  private Player Player => GameManager.S.Player;

  private void Awake() {
    animators.Add(idleGrounded);
    animators.Add(midair);
    animators.Add(running);
  }

  private void Update() {
    animators.ForEach(a => DisableAnimator(a));
    animators.ForEach(a => a.FlipX = Player.XVelocity > 0 ? false : Player.XVelocity < 0 ? true : a.FlipX);
    animators.ForEach(a => a.FlipY = Player.IsStunned);

    if (Player.YVelocity != 0) {
      EnableAnimator(midair);

    } else if (Player.XVelocity != 0) {
      EnableAnimator(running);

    } else {
      EnableAnimator(idleGrounded);
    }
  }

  private void DisableAnimator(ISpriteAnimator animator) {
    animator.IsPaused = true;
    animator.IsVisible = false;
  }


  private void EnableAnimator(ISpriteAnimator animator) {
    animator.IsPaused = false;
    animator.IsVisible = true;
    if (current != animator) {
      animator.StartFromFirstFrame();
    }
    current = animator;
  }
}
