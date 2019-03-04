using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTileEntityObject : MonoBehaviour {
  [Header("INITIALIZATION ONLY")]
  public int spawnRow;
  public int spawnCol;

  public virtual void SetPosition(int row, int col) {
    Vector3 newPosition = GameManager.S.Board[row, col].transform.position;
    newPosition.z = transform.position.z;
    transform.position = newPosition;
  }
}
