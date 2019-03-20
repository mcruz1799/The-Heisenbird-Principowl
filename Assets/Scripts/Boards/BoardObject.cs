using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class BoardObject : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private TextAsset level;
  [SerializeField] private ASCIIToTileInhabitantMaker[] asciiMappings;
#pragma warning restore 0649

  public Tile tilePrefab;

  public int NumRows { get; private set; }
  public int NumCols { get; private set; }

  private char[,] levelChars;

  public void Initialize() {
    //Calculate the number of rows and columns
    string[] lines = level.text.Split('\n');
    NumRows = lines.Length;
    NumCols = 1;
    for (int i = 0; i < NumRows; i++) {
      lines[i] = lines[i].Trim();
      if (lines[i].Length > NumCols) {
        NumCols = lines[i].Length;
      }
    }

    levelChars = new char[NumCols, NumRows];
    int row = 0;
    foreach (string line in lines) {
      int col = 0;
      foreach (char c in line) {
        levelChars[col, NumRows - row - 1] = c;
        col += 1;
      }
      row += 1;
    }
  }

  //Fills GameManager.S.Board with tile inhabitants
  public void PopulateBoard() {
    for (int r = 0; r < NumRows; r++) {
      for (int c = 0; c < NumCols; c++) {
        foreach (ASCIIToTileInhabitantMaker asciiMapping in asciiMappings) {
          if (asciiMapping.ascii == levelChars[c, r]) {
            //Debug.LogFormat("{0} {1} {2}", r, c, levelChars[c, r]);
            asciiMapping.maker.Make(r, c, GameManager.S.TileInhabitantObjectHolder);
          }
        }
      }
    }
  }

  ////Use the color mapping for a particular level to populate the tiles.
  //private void PopulateTileFromMap(int row, int col) {

  //  Color pixelColor;
  //  try {
  //    pixelColor = level.GetPixel(x0 + col * pixelWidth, y0 + row * pixelHeight);
  //  } catch (System.Exception e) {
  //    throw new System.Exception("Failed to obtain Pixel. Error: " + e);
  //  }

  //  //If the pixel isn't transparent...
  //  if (pixelColor.a != 0) {
  //    foreach (ColorToTileInhabitantMaker colorMapping in colorMappings) {
  //      if (colorMapping.color.Equals(pixelColor)) {
  //        colorMapping.maker.Make(row, col, GameManager.S.TileInhabitantObjectHolder);
  //      }
  //    }
  //  }
  //}

    //Use the color mapping for a particular level to populate the tiles.
  //private void PopulateTileFromMapASCII(int row, int col) {
  //  if (ascii == null) InitializeASCIIArray();

  //  char currentChar;
  //  try {
  //    currentChar = ascii[row][col];
  //  } catch (System.Exception e) {
  //    throw new System.Exception("Failed to obtain Character. Error: " + e);
  //  }

  //  foreach (ASCIIToTileInhabitantMaker asciiMapping in asciiMappings) {
  //    if (asciiMapping.ascii.Equals(currentChar)) {
  //      asciiMapping.maker.Make(row, col, GameManager.S.TileInhabitantObjectHolder);
  //    }
  //  }
  //}

  //private void InitializeASCIIArray()
  //{
  //  ascii = new List<char[]>();
  //  string fs = level.text;
  //  string[] fLines = Regex.Split(fs, "\n|\r|\r\n");

  //  for (int i = fLines.Length - 1; i >= 0; i--) { //Need to reverse so file is read from bottom to top.
  //    string s = fLines[i];
  //    char[] chars = s.ToCharArray();
  //    ascii.Add(chars);
  //  }
  //}


}