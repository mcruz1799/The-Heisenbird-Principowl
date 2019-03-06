using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour {
#pragma warning disable 0649
  [Range(1, 300)] public int numRows = 1, numCols = 1;
  public Tile tilePrefab;
  public ColorToTileInhabitantMaker[] colorMappings;
  public Texture2D level;
  public int x0, y0;
  [Range(1, 100)] public int pixelWidth = 1, pixelHeight = 1;
#pragma warning restore 0649

  //Fills GameManager.S.Board with tile inhabitants
  public void PopulateBoard() {
    //TODO: Clear GameManager.S.Board of tile inhabitants?
    //  How to handle the Player...

    for (int row = 0; row < numRows; row++) {
      for (int col = 0; col < numCols; col++) {
        PopulateTileFromMap(row, col);
      }
    }
  }

  //Use the color mapping for a particular level to populate the tiles.
  private void PopulateTileFromMap(int row, int col) {

    Color pixelColor;
    try {
      pixelColor = level.GetPixel(x0 + col * pixelWidth, y0 + row * pixelHeight);
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