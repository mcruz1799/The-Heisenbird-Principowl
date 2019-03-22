using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableTile : SingleTileEntity, ITurnTaker
{

  private readonly FlammableTileObject gameObject;
  private bool isOnFire;

  public FlammableTile(FlammableTileObject gameObject) : base(gameObject) {
    this.gameObject = gameObject;
    this.isOnFire = false;
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
      ITileInhabitant follower = this;
      foreach (ITileInhabitant habiter in inhabitants){
        if (habiter is FollowerEnemy){
          follower = habiter;
          isOnFire = true;
          for (int i = 1; i <= gameObject.numUpdraftTiles; i++){
            IReadOnlyCollection<ITileInhabitant> updrafts = GameManager.S.Board[Row + i, Col].Inhabitants;
            foreach (ITileInhabitant tile in updrafts){
              if (tile is UpdraftTile){
                UpdraftTile ud = (UpdraftTile) tile;
                ud.IsActive = true;
              }
            }
          }
        }
      }
      if (follower != this) ((FollowerEnemy)follower).Destroy();
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
