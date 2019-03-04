using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileInhabitantMaker : MonoBehaviour {
  public abstract ITileInhabitant Make(int row, int col, Transform parent = null);
}
