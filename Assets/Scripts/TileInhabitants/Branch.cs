using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : Platform
{
  protected override int platformLength 
  {
    get { return 3; } //Replace with proper branch length.
    set { platformLength = value; }
  }

  protected override string Color => "";

  protected override System.Tuple<Dictionary<SingleTileEntity, Vector2Int>, SingleTileEntity> ConstructSelf()
  {
    Dictionary<SingleTileEntity, Vector2Int> self = new Dictionary<SingleTileEntity, Vector2Int>();
    SingleTileEntity leading = null;
    for (int i = 0; i < platformLength; i++) {
      GameObject g = new GameObject();
      Wall w = g.AddComponent<Wall>();
      w.transform.parent = this.transform;
      w.transform.localPosition = new Vector3(0, 0, -0.1f);
      Debug.Log("Row and Col: " + w.Row + " " + w.Col);
      self[w] = new Vector2Int(i, 0);
      if (i == 0) leading = w;
    }
    System.Tuple<Dictionary<SingleTileEntity, Vector2Int>, SingleTileEntity> tuple = new System.Tuple<Dictionary<SingleTileEntity, Vector2Int>, SingleTileEntity>(self, leading);
    return tuple;
  }

}
