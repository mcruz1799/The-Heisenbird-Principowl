using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
  [SerializeField] private Player player;

  [SerializeField] private SpriteAnimator idleGrounded;
  [SerializeField] private SpriteAnimator midair;
}
