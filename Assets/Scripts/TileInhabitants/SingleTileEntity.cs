using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleTileEntity : ITileInhabitant {

  //Exists as a hack to simplify how we prevent MultiTileEntity from colliding with itself
  public readonly ISet<ITileInhabitant> toIgnore = new HashSet<ITileInhabitant>();

  private readonly SingleTileEntityObject gameObject;

  public int Row { get; private set; }
  public int Col { get; private set; }

  public SingleTileEntity(SingleTileEntityObject gameObject) {
    this.gameObject = gameObject;
    SetPosition(gameObject.spawnRow, gameObject.spawnCol, out bool success);
    if (!success) {
      throw new System.Exception("Failed to initialize SingleTileEntity");
    }
  }

  //Code taken from https://stackoverflow.com/questions/11678693/all-cases-covered-bresenhams-line-algorithm
  private static List<Vector2Int> BresenhamLineAlgorithm(int x0, int y0, int xf, int yf) {
    List<Vector2Int> result = new List<Vector2Int>();

    int w = xf - x0;
    int h = yf - y0;

    int dx1 = System.Math.Sign(w);
    int dy1 = System.Math.Sign(h);
    int dx2 = dx1;
    int dy2 = 0;

    int longest = Mathf.Abs(w);
    int shortest = Mathf.Abs(h);
    if (!(longest > shortest)) {
      longest = Mathf.Abs(h);
      shortest = Mathf.Abs(w);
      dy2 = System.Math.Sign(h);
      dx2 = 0;
    }

    int numerator = longest >> 1;
    for (int i = 0; i <= longest; i++) {
      result.Add(new Vector2Int(x0, y0));
      numerator += shortest;
      if (!(numerator < longest)) {
        numerator -= longest;
        x0 += dx1;
        y0 += dy1;
      } else {
        x0 += dx2;
        y0 += dy2;
      }
    }

    return result;
  }

  //Calculate the waypoints for moving in a line from GameBoard[Row, Col] to GameBoard[Row + yDelta, Col + xDelta]
  //Note that the returned waypoints are xy-coordinates, NOT row-col-pairs
  //Ensures that each waypoint shares an EDGE (not just a corner) with the next.
  protected List<Vector2Int> CalculateMoveWaypoints(int xDelta, int yDelta) {
    //Debug.LogFormat("x0: {0}  y0: {1}    xf: {2}  yf: {3}", Col, Row, Col + xDelta, Row + yDelta);
    List<Vector2Int> waypoints = BresenhamLineAlgorithm(Col, Row, Col + xDelta, Row + yDelta);

    int row = Row, col = Col;
    List<Vector2Int> result = new List<Vector2Int>();
    int maxIterations = 100;
    for (int i = 0; i < waypoints.Count && maxIterations > 0; i++) {
      maxIterations -= 1;
      Vector2Int waypoint = waypoints[i];
      int newRow = waypoint.y;
      int newCol = waypoint.x;
      int xDir = newCol - col;
      int yDir = newRow - row;

      //If the waypoint is diagonal, add an intermediate waypoint
      if (xDir != 0 && yDir != 0) {
        //Create a new waypoint by undoing the change in the x-direction
        waypoint = new Vector2Int(newCol - xDir, newRow);
        newRow = waypoint.y;
        newCol = waypoint.x;
        xDir = newCol - col;
        yDir = newRow - row;
        i -= 1; //We want to visit this waypoint again on the next iteration

        if (xDir != 0 && yDir != 0) {
          Debug.LogError("Diagonal waypoint error in movement calculations");
        }
      }

      row = newRow;
      col = newCol;
      result.Add(waypoint);
    }

    if (maxIterations == 0) {
      Debug.LogWarning("Problem in CalculateMoveWaypoints");
    }

    //string s = "";
    //result.ForEach(v => s += " " + v.ToString());
    //Debug.Log(s);

    //Debug-check.  Do the waypoints start and end at the right positions?
    if (result[0].y != Row || result[0].x != Col || result[result.Count-1].y != Row + yDelta || result[result.Count-1].x != Col + xDelta) {
      Debug.LogError("Error in waypoint computation");
    }

    return result;
  }

  public bool CanSetPosition(int newRow, int newCol) {
    return GameManager.S.Board.IsPositionLegal(newRow, newCol) && GameManager.S.Board[newRow, newCol].CanAdd(this);
  }

  public void SetPosition(int newRow, int newCol, out bool success) {
    success = CanSetPosition(newRow, newCol);
    if (success) {
      GameManager.S.Board[Row, Col].Remove(this);
      GameManager.S.Board[newRow, newCol].Add(this);
      Row = newRow;
      Col = newCol;
      gameObject.SetPosition(Row, Col);
    }
  }

  //
  //ITileInhabitant
  //

  public bool IsBlockedBy(ITileInhabitant other) {
    return !toIgnore.Contains(other) && IsBlockedByCore(other);
  }

  protected abstract bool IsBlockedByCore(ITileInhabitant other);

  public ISet<Tile> Occupies() {
    return new HashSet<Tile>() { GameManager.S.Board[Row, Col] };
  }
}
