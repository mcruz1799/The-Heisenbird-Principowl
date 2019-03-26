using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private SpriteAnimator idle;
#pragma warning restore 0649

  public IEnemy enemy;

  private void Start() {
    idle.StartFromFirstFrame();
    if (enemy == null) {
      //enemy field is set in the Enemy script
      throw new System.Exception("EnemyAnimator isn't initialized.  Enemy field needs to be set.");
    }
  }

  private void LateUpdate() {
    if (enemy.XVelocity > 0) {
      idle.FlipX = false;

    } else if (enemy.XVelocity < 0) {
      idle.FlipX = true;

    }
  }
}
