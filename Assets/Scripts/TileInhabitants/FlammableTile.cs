using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableTile : SingleTileEntity, ITurnTaker {
  private readonly FlammableTileObject gameObject;
  private bool isOnFire;

  private UpdraftTileMaker updraftTileMaker;

  public FlammableTile(FlammableTileObject gameObject) : base(gameObject) {
    this.gameObject = gameObject;
    this.isOnFire = false;
    this.updraftTileMaker = gameObject.updraftTileMaker;
    GameManager.S.RegisterTurnTaker(this);
  }

  protected override bool IsBlockedByCore(ITileInhabitant other){
    /*if (other is Platform) {
      Platform platform = (Platform)other;
      return platform.IsActive;
    }*/
    return false;
  }

  public void OnTurn() {
    if (GetWisp()){
      SetOnFire();
    }
  }

  private void SetOnFire(){
    if (isOnFire) return;
    isOnFire = true;
    ActivateAdjacentTiles();
    MakeUpdrafts();
  }

  private bool GetWisp(){
    IReadOnlyCollection<ITileInhabitant> inhabitants = GameManager.S.Board[Row, Col].Inhabitants;
      ITileInhabitant follower = this;
      foreach (ITileInhabitant habiter in inhabitants){
        if (habiter is FollowerEnemySubEntity){
          follower = habiter;
          break;
        }
      }
      if (follower != this){
        ((FollowerEnemySubEntity)follower).OnAttacked(100000, Direction.West);
        return true;
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
          flammableTile.SetOnFire();
        }
      }
    }
  }

  private void MakeUpdrafts(){
    for (int i = 1; i <= gameObject.numUpdraftTiles; i++){
      updraftTileMaker.Make(Row + i, Col);
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
