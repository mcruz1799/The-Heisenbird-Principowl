using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableTile : SingleTileEntity, ITurnTaker {
  private readonly FlammableTileObject gameObject;
  private readonly UpdraftTileMaker updraftTileMaker;

  public FlammableTile(FlammableTileObject gameObject) : base(gameObject) {
    this.gameObject = gameObject;
    updraftTileMaker = gameObject.updraftTileMaker;
    GameManager.S.RegisterTurnTaker(this);
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

  private bool isOnFire = false;
  private bool hasCreatedUpdrafts = false;
  public void OnTurn() {
    //If not on fire, check for wisps.  If one is found, set self on fire
    if (!isOnFire && GetWisp()){
      isOnFire = true;
    }

    //If on fire, 10% chance to set adjacent FlammableTiles on fire
    if (isOnFire && Random.Range(0, 10) == 0) {
      ActivateAdjacentTiles();
    }

    //Create updrafts if on fire and we haven't done so yet
    if (isOnFire && !hasCreatedUpdrafts) {
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
          flammableTile.isOnFire = true;
        }
      }
    }
  }

  private void MakeUpdrafts() {
    int row = Row;
    while (CanSetPosition(row, Col)) {
      if (Row - row >= gameObject.numUpdraftTiles) {
        break;
      }
      row += 1;
      updraftTileMaker.Make(row, Col);
    }
  }

  public static FlammableTile Make(FlammableTileObject flammableTilePrefab, int row, int col, Transform parent = null) {
    flammableTilePrefab = Object.Instantiate(flammableTilePrefab);
    flammableTilePrefab.transform.parent = parent;
    flammableTilePrefab.spawnRow = row;
    flammableTilePrefab.spawnCol = col;
    return new FlammableTile(flammableTilePrefab);
  }
}
