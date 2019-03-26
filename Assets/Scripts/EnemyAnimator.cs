using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private SpriteAnimator idle;
  private IEnemy enemy;

#pragma warning restore 0649
  private void Awake(){
    idle.StartFromFirstFrame();
    enemy = transform.parent.GetComponent<IEnemy>();
  }

  private void LateUpdate() {
    if (enemy.XVelocity > 0) {
      idle.FlipX = false;

    } else if (enemy.XVelocity < 0) {
      idle.FlipX = true;

    }
  }
}
