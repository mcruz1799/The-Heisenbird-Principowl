using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableTile : SingleTileEntity, ITurnTaker
{

  private readonly FlammableTileObject gameObject;
  private UpdraftTileMaker updraftTileMaker;
  private bool isOnFire;

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
      IReadOnlyCollection<ITileInhabitant> inhabitants = GameManager.S.Board[Row, Col].Inhabitants;
      foreach (ITileInhabitant habiter in inhabitants){
        if (habiter is FollowerEnemy){
          FollowerEnemy follower = (FollowerEnemy) habiter;
          isOnFire = true;
          follower.Destroy();
          for (int i = 1; i <= gameObject.numUpdraftTiles; i++){
            updraftTileMaker.Make(Row + i, Col, null);
          }
        }
      }
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
