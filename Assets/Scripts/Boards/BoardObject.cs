using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour {
#pragma warning disable 0649
  [Range(1, 300)] public int numRows = 1, numCols = 1;
  public Tile tilePrefab;
  public ColorToTileInhabitantMaker[] colorMappings;
  public Texture2D[] levels;
#pragma warning restore 0649

  //Fills GameManager.S.Board with tile inhabitants
  public void PopulateBoard(int level = 0) {
    //TODO: Clear GameManager.S.Board of tile inhabitants?
    //  How to handle the Player...

    for (int row = 0; row < numRows; row++) {
      for (int col = 0; col < numCols; col++) {
        PopulateTileFromMap(row, col, level);
      }
    }
  }

  //Use the color mapping for a particular level to populate the tiles.
  private void PopulateTileFromMap(int row, int col, int level = 0) {
    Texture2D map = levels[level];

    Color pixelColor;
    try {
      pixelColor = map.GetPixel(col, row);
    } catch (System.Exception e) {
      throw new System.Exception("Failed to obtain Pixel. Error: " + e);
    }

    //If the pixel isn't transparent...
    if (pixelColor.a != 0) {
      foreach (ColorToTileInhabitantMaker colorMapping in colorMappings) {
        if (colorMapping.color.Equals(pixelColor)) {
          colorMapping.maker.Make(row, col, GameManager.S.TileInhabitantObjectHolder);
        }
      }
    }
  }
}