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
    if (!isOnFire){
      if (GetWisp()){
        isOnFire = true;
        //set nearby tiles on fire
        ActivateAdjacentTiles();
      }
    }
      
  }

  private bool GetWisp(){
    IReadOnlyCollection<ITileInhabitant> inhabitants = GameManager.S.Board[Row, Col].Inhabitants;
      ITileInhabitant follower = this;
      foreach (ITileInhabitant habiter in inhabitants){
        if (habiter is FollowerEnemy){
          follower = habiter;
        }
      }
      if (follower != this){
        ((FollowerEnemy)follower).Destroy();
        return true;
      }
      return false;
  }

  private void ActivateAdjacentTiles(){
    for(int i = 1; i <= gameObject.numFireTiles; i++){
      IReadOnlyCollection<ITileInhabitant> inhabitants = GameManager.S.Board[Row, Col + i].Inhabitants;
      foreach (ITileInhabitant habiter in inhabitants){
        if (habiter is FlammableTile){
          MakeUpdrafts(Row, Col + i);
        }
      }
    }
  }

  private void MakeUpdrafts(int Row, int Col){
    for (int i = 1; i <= gameObject.numUpdraftTiles; i++){
      //set tiles above this true
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
