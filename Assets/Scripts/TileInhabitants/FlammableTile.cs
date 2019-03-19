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
    SetPosition(gameObject.spawnRow, gameObject.spawnCol, out bool success);
    if (!success) {
      throw new System.Exception("Failed to initialize FlammableTile");
    }
  }

  protected override bool IsBlockedByCore(ITileInhabitant other){
    if (other is Platform) {
      Platform platform = (Platform)other;
      return platform.IsActive;
    }
    return false;
  }

  public void OnTurn() {
    if (!isOnFire){
      IReadOnlyCollection<ITileInhabitant> inhabitants = GameManager.S.Board[Row, Col].Inhabitants;
      foreach (ITileInhabitant habiter in inhabitants){
        if (habiter is FollowerEnemy){
          isOnFire = true;
          //habiter.TakeDamage(1);
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