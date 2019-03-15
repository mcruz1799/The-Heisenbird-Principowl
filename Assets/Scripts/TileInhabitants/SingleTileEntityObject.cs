using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTileEntityObject : MonoBehaviour {
  [Header("INITIALIZATION ONLY")]

  //When constructing a SingleTileEntity, SetPosition(spawnRow, spawnCol) gets called.
  public int spawnRow;
  public int spawnCol;

  private Coroutine movementCoroutine;

  public void SetPosition(int row, int col) {
    Vector3 newPosition = GameManager.S.Board[row, col].transform.position;
    newPosition.z = transform.position.z;
    // transform.position = newPosition;
    if (movementCoroutine != null) {
      StopCoroutine(movementCoroutine);
    }
    movementCoroutine = StartCoroutine(MoveToPosition(transform, newPosition, GameManager.S.TimeBetweenTurns));
  }

  private IEnumerator MoveToPosition(Transform transform, Vector3 position, float timeToMove) {
    var currentPos = transform.position;
    var t = 0f;
    while (t < 1) {
      t += Time.deltaTime / timeToMove;
      transform.position = Vector3.Lerp(currentPos, position, t);
      yield return null;
    }
  }
}
