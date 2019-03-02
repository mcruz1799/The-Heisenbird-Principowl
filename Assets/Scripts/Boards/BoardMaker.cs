using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMaker : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private Vector2Int playerSpawn;
  [SerializeField] private GameObject wallPrefab;
  [SerializeField] private ColorToPrefab[] colorMappings;
  [SerializeField] private Texture2D[] levels;
#pragma warning restore 0649

  public ISet<ITileInhabitant> PopulateTile(int row, int col) {
    //return PopulateTile1(row, col);
    return PopulateTileFromMap(row, col);
  }

  private ISet<ITileInhabitant> PopulateTile1(int row, int col) {
    ISet<ITileInhabitant> result = new HashSet<ITileInhabitant>();

    if (row == 0) {
      Wall wall = Instantiate(wallPrefab.GetComponentInChildren<Wall>());
      result.Add(wall);
    }

    if ((row == playerSpawn.y) && (col == playerSpawn.x)) {
      GameManager.S.Player.SetPosition(row, col, out bool success);
      if (!success) {
        throw new System.Exception("Failed to place player");
      }
    }

    return result;
  }

  //Using the color mapping for a particular level to populate the tiles.
  private ISet<ITileInhabitant> PopulateTileFromMap(int row, int col, int level = 0)
  {
    ISet<ITileInhabitant> result = new HashSet<ITileInhabitant>();
    Texture2D map = levels[level];

    Color pixelColor;
    try {
      pixelColor = map.GetPixel(col, row);
    } catch (System.Exception e) {
      Debug.Log("Failed to obtain Pixel. Error: " + e);
      return result;
    }
    

    if (pixelColor.a == 0) {
      // The pixel is transparrent. Let's ignore it!
      return result;
    } else if ((row == playerSpawn.y) && (col == playerSpawn.x)) {
      //TODO: Decide whether player spawn needs to be changed per level.
      GameManager.S.Player.SetPosition(row, col, out bool success);
      if (!success) {
        throw new System.Exception("Failed to place player");
      }
      return result;
    }

      //Placing Non-Player Inhabitants
    else {
      foreach (ColorToPrefab colorMapping in colorMappings) {
        if (colorMapping.color.Equals(pixelColor)) {
          Debug.Log("Found Wall.");
          GameObject g = Instantiate(colorMapping.prefab);
          ITileInhabitant inhabitant = g.GetComponent<ITileInhabitant>();
          result.Add(inhabitant);
        }
      }
    }
    return result;
  }
}
