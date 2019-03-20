using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class BoardObject : MonoBehaviour {
#pragma warning disable 0649
  [Range(1, 300)] public int numRows = 1, numCols = 1;
  public Tile tilePrefab;
  public ColorToTileInhabitantMaker[] colorMappings;
  public ASCIIToTileInhabitantMaker[] asciiMappings;
  public Texture2D level;
  public TextAsset levelAscii;
  private List<char[]> ascii;
  public int x0, y0;
  [Range(1, 100)] public int pixelWidth = 1, pixelHeight = 1;
#pragma warning restore 0649

  //Fills GameManager.S.Board with tile inhabitants
  public void PopulateBoard() {
    //TODO: Clear GameManager.S.Board of tile inhabitants?
    //  How to handle the Player...

    for (int row = 0; row < numRows; row++) {
      for (int col = 0; col < numCols; col++) {
        PopulateTileFromMapASCII(row, col);
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

  //Use the color mapping for a particular level to populate the tiles.
  private void PopulateTileFromMapASCII(int row, int col)
  {
    if (ascii == null) initializeASCIIArray();

    char currentChar;
    try {
      currentChar = ascii[row][col];
    } catch (System.Exception e) {
      //Debug.Log("Failed to obtain Character. Error: " + e); //TODO: Better alternative than just ignoring failed inputs?
      return;
    }

    foreach (ASCIIToTileInhabitantMaker asciiMapping in asciiMappings) {
      if (asciiMapping.ascii.Equals(currentChar)) {
        asciiMapping.maker.Make(row, col, GameManager.S.TileInhabitantObjectHolder);
      }
    }
  }
  

  private void initializeASCIIArray()
  {
    Debug.Log("Initialized.");
    string fs = levelAscii.text;
    string[] fLines = Regex.Split(fs, "\n|\r|\r\n");
    ascii = new List<char[]>();
    for (int i = fLines.Length - 1; i >= 0; i--) { //Need to reverse so file is read from bottom to top.
      string s = fLines[i];
      char[] chars = s.ToCharArray();
      if (chars.Length > 0) ascii.Add(chars);
    }
  }


}