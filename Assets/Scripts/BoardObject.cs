using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

//Contains information for initializing a board
//Configurable via the Unity inspector
public class BoardObject : MonoBehaviour {
#pragma warning disable 0649
  public Tile tilePrefab;
  [SerializeField] private ASCIIToTileInhabitantMaker[] asciiMappings;
#pragma warning restore 0649

  public int NumRows { get; private set; }
  public int NumCols { get; private set; }

  private char[,] levelChars;

  private void Start() {
    PopulateBoard();
  }

  public void Initialize(TextAsset level) {
    //Calculate the number of rows and columns
    string[] lines = level.text.Split('\n');
    NumRows = lines.Length;
    NumCols = 1;
    for (int i = 0; i < NumRows; i++) {
      lines[i] = lines[i].TrimEnd();
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
            asciiMapping.maker.Make(r, c, GameManager.S.TileInhabitantObjectHolder);
          }
        }
      }
    }
  }
}