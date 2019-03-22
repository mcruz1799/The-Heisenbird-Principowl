using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileInhabitantMaker : MonoBehaviour {
  public abstract void Make(int row, int col, Transform parent = null);
}

public abstract class EnemyMaker : TileInhabitantMaker {
  public abstract IEnemy MostRecentlyMade { get; }
}