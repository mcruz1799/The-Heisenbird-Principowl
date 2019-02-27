using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private BoardMaker boardMaker;
  [SerializeField] private Tile tilePrefab;
  [Range(1, 200)] [SerializeField] private int numRows = 1, numCols = 1;
#pragma warning restore 0649

  private Tile[,] board;

  public int Rows { get; private set; }
  public int Cols { get; private set; }

  private void Start() {
    Rows = numRows;
    Cols = numCols;
    board = new Tile[Rows, Cols];

    float tileWidth = tilePrefab.transform.lossyScale.x;
    float tileHeight = tilePrefab.transform.lossyScale.y;
    float xOffset = -tileWidth / 2f * Cols;
    float yOffset = -tileHeight / 2f * Rows;
    for (int row = 0; row < Rows; row++) {
      for (int col = 0; col < Cols; col++) {
        board[row, col] = Instantiate(original: tilePrefab, parent: transform);
        board[row, col].transform.position = new Vector3(tileWidth * col + xOffset, tileHeight * row + yOffset, 0);
        ISet<ITileInhabitant> inhabitants = boardMaker.PopulateTile(row, col);
        foreach (ITileInhabitant inhabitant in inhabitants) {
          inhabitant.SetPosition(row, col, out bool success);
          if (!success) {
            throw new System.Exception("Failed to initialize Board");
          }
        }
      }
    }
  }
  
  //Returns whether the given position is a legal position on the board
  public bool IsPositionLegal(int row, int col) {
    bool legalRow = 0 <= row && row < Rows;
    bool legalCol = 0 <= col && col < Cols;

    return legalRow && legalCol;
  }

  public Tile this[int row, int col] {
    get => board[row, col];
  }
}
