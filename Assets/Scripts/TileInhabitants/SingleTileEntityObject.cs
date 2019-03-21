using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTileEntityObject : MonoBehaviour {

  //When constructing a SingleTileEntity, SetPosition(spawnRow, spawnCol) gets called.
  [HideInInspector] public int spawnRow;
  [HideInInspector] public int spawnCol;
  public bool animateMovement = true;
  protected virtual float MoveAnimationTime => GameManager.S.TimeBetweenTurns;

  private Coroutine movementCoroutine;

  public void SetPosition(int row, int col) {
    Vector3 newPosition = GameManager.S.Board[row, col].transform.position;
    newPosition.z = transform.position.z;
    if (animateMovement) {
      if (movementCoroutine != null) {
        StopCoroutine(movementCoroutine);
      }
      movementCoroutine = StartCoroutine(MoveToPosition(transform, newPosition, MoveAnimationTime));
    } else {
      transform.position = newPosition;
    }
  }

  private IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove) {
    Vector3 currentPos = transform.position;
    float t = 0f;
    while (t < 1) {
      yield return null;
      t += Time.deltaTime / timeToMove;
      transform.position = Vector3.Lerp(currentPos, position, t);
    }
  }
}
