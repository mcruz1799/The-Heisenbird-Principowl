﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A board of tiles, indexed by [row, col]
public class Board {
  private readonly Tile[,] board;

  public int Rows { get; }
  public int Cols { get; }

  public Board(int numRows, int numCols, Tile tilePrefab, Transform tileHolder) {
    Rows = numRows;
    Cols = numCols;
    board = new Tile[Rows, Cols];

    float tileWidth = tilePrefab.transform.lossyScale.x;
    float tileHeight = tilePrefab.transform.lossyScale.y;
    float xOffset = -tileWidth / 2f * Cols;
    float yOffset = -tileHeight / 2f * Rows;
    for (int row = 0; row < Rows; row++) {
      for (int col = 0; col < Cols; col++) {
        //Tile t = Object.Instantiate(tilePrefab);
        //t.transform.parent = tileHolder;
        board[row, col] = Object.Instantiate(original: tilePrefab, parent: tileHolder);
        board[row, col].transform.position = new Vector3(tileWidth * col + xOffset, tileHeight * row + yOffset, 0);
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

  //Get a Tile N/S/E/W of Board[row, col].
  //Returns null if there is no Tile in that direction
  public Tile GetInDirection(int row, int col, Direction direction) {
    switch (direction) {
      case Direction.North:
        return IsPositionLegal(row + 1, col) ? this[row + 1, col] : null;

      case Direction.South:
        return IsPositionLegal(row - 1, col) ? this[row - 1, col] : null;

      case Direction.East:
        return IsPositionLegal(row, col + 1) ? this[row, col + 1] : null;

      case Direction.West:
        return IsPositionLegal(row, col - 1) ? this[row, col - 1] : null;

      default:
        throw new System.ArgumentException("Illegal direction enum value");
    }
  }

  public Tile GetInDirections(int row, int col, params Direction[] directions) {
    foreach (Direction d in directions) {
      switch (d) {
        case Direction.North: row += 1; break;
        case Direction.South: row -= 1; break;
        case Direction.East:  col += 1; break;
        case Direction.West:  col -= 1; break;

        default: throw new System.ArgumentException("Illegal direction enum value");
      }
    }
    if (IsPositionLegal(row, col)) {
      return this[row, col];
    } else {
      return null;
    }
  }
}
