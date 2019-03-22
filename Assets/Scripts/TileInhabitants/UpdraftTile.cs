using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdraftTile : SingleTileEntity
{
  private readonly UpdraftTileObject updraftObject;

  public bool IsActive {
    get => updraftObject.isActive;
    set => updraftObject.isActive = value;
  }

  public UpdraftTile(UpdraftTileObject updraftObject) : base(updraftObject) {
    this.updraftObject = updraftObject;
    this.IsActive = false;
  }

  protected override bool IsBlockedByCore(ITileInhabitant other){
    if (other is Platform) {
      Platform platform = (Platform)other;
      return platform.IsActive;
    }
    return false;
  }

  public static UpdraftTile Make(UpdraftTileObject updraftTilePrefab, int row, int col, Transform parent = null) {
    updraftTilePrefab = Object.Instantiate(updraftTilePrefab);
    updraftTilePrefab.transform.parent = parent;
    updraftTilePrefab.spawnRow = row;
    updraftTilePrefab.spawnCol = col;
    return new UpdraftTile(updraftTilePrefab);
  }
}
