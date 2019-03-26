using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileInhabitantMaker : MonoBehaviour {
  public abstract object Make(int row, int col, Transform parent = null);
}

public abstract class TileInhabitantMaker<T> : TileInhabitantMaker {
  public abstract T MakeAndGet(int row, int col, Transform parent = null);

  public override object Make(int row, int col, Transform parent = null) {
    return MakeAndGet(row, col, parent);
  }
}
