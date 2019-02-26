using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : ITileInhabitant {
  private readonly Board board;
  private readonly int row, col;

  public Wall(Board board, int row, int col) {
    this.board = board;
    this.row = row;
    this.col = col;

    board[row, col].Add(this);
  }

  //A Wall cannot be placed in a Tile that has any other occupant
  public bool IsBlockedBy(ITileInhabitant other) {
    return true;
  }

  public ISet<Tile> Occupies() {
    return new HashSet<Tile>() { board[row, col] };
  }
}
