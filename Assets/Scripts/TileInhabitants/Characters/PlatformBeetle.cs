using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeetle : Enemy
{
  private readonly PlatformBeetleObject e;
  private Direction currentDirection;

  public PlatformBeetle(SingleTileEntityObject e) : base((EnemyObject)e)
  {
    //TODO: Add Other Necessary Setup Here.
    this.e = (PlatformBeetleObject)e;
    this.currentDirection = Direction.East;
  }

  protected override int SpeedX {
    get => e.xSpeed;
    set => e.xSpeed = value;
  }
  protected override int SpeedY {
    get => e.xSpeed;
    set => e.xSpeed = value;
  }

  protected override Direction Facing {
    get => this.currentDirection;
    set => this.currentDirection = value;
  }

  public static PlatformBeetle Make(SingleTileEntityObject platformBeetlePrefab, int row, int col, Transform parent = null)
  {
    GameObject g = platformBeetlePrefab.gameObject.transform.parent.gameObject;
    GameObject instance = Object.Instantiate(g);
    platformBeetlePrefab = instance.GetComponentInChildren<SingleTileEntityObject>();
    instance.transform.parent = parent;
    platformBeetlePrefab.spawnRow = row;
    platformBeetlePrefab.spawnCol = col;
    return new PlatformBeetle(platformBeetlePrefab);
  }
}
