using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableTile : SingleTileEntity, ITurnTaker {
  private readonly FlammableTileObject gameObject;
  private readonly UpdraftTileMaker updraftTileMaker;

  public bool IsOnFire { get; private set; } = false;

  private FlammableTile(FlammableTileObject gameObject, out bool success) : base(gameObject, out success) {
    if (success) {
      this.gameObject = gameObject;
      updraftTileMaker = gameObject.updraftTileMaker;
      GameManager.S.RegisterTurnTaker(this);
    }
  }

  public override bool CanSetPosition(int newRow, int newCol) {
    if (!base.CanSetPosition(newRow, newCol)) {
      return false;
    }

    foreach (ITileInhabitant other in GameManager.S.Board[newRow, newCol].Inhabitants) {
      if (toIgnore.Contains(other)) {
        continue;
      }

      if (other is Platform) {
        return false;
      }
    }

    return true;
  }

  private bool hasCreatedUpdrafts = false;
  public void OnTurn() {
    //If not on fire, check for wisps.  If one is found, set self on fire
    if (!IsOnFire && GetWisp()){
      IsOnFire = true;
    }

    //If on fire, 10% chance to set adjacent FlammableTiles on fire
    if (IsOnFire && Random.Range(0, 10) == 0) {
      ActivateAdjacentTiles();
    }

    //Create updrafts if on fire and we haven't done so yet
    if (IsOnFire && !hasCreatedUpdrafts) {
      MakeUpdrafts();
      hasCreatedUpdrafts = true;
    }
  }

  private bool GetWisp(){
    foreach (ITileInhabitant other in GameManager.S.Board[Row, Col].Inhabitants) {
      if (toIgnore.Contains(other)) {
        continue;
      }

      FollowerEnemySubEntity wisp = other is FollowerEnemySubEntity ? (FollowerEnemySubEntity)other : null;
      if (wisp != null) {
        wisp.OnAttacked(int.MaxValue, Direction.East);
        return true;
      }
    }
    return false;
  }

  private void ActivateAdjacentTiles(){
    foreach (Direction d in System.Enum.GetValues(typeof(Direction))) {
      Tile adjacent = GameManager.S.Board.GetInDirection(Row, Col, d);
      if (adjacent == null) {
        continue;
      }
      foreach (ITileInhabitant inhabitant in adjacent.Inhabitants) {
        FlammableTile flammableTile = inhabitant is FlammableTile ? (FlammableTile)inhabitant : null;
        if (flammableTile != null) {
          flammableTile.IsOnFire = true;
        }
      }
    }
  }

  private void MakeUpdrafts() {
    int row = Row;
    while (true) {
      if (row - Row >= gameObject.numUpdraftTiles) {
        break;
      }
      row += 1;
      if (updraftTileMaker.Make(row, Col) == null) {
        break;
      }
    }
  }

  public static FlammableTile Make(FlammableTileObject flammableTilePrefab, int row, int col, Transform parent = null) {
    flammableTilePrefab = Object.Instantiate(flammableTilePrefab);
    flammableTilePrefab.transform.parent = parent;
    flammableTilePrefab.spawnRow = row;
    flammableTilePrefab.spawnCol = col;
    FlammableTile result = new FlammableTile(flammableTilePrefab, out bool success);
    return success ? result : null;
  }
}
