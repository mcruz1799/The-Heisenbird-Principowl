using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBeetle : Enemy {
  private readonly PlatformBeetleObject e;

  protected override Direction AttackDirection => XVelocity > 0 ? Direction.East : Direction.West;
  public PlatformAndBeetleColor GroupColor => e.groupColor;

  private PlatformBeetle(PlatformBeetleObject e) : base(e) {
    this.e = e;
    XVelocity = 1;
  }

  public override void OnTurn() {
    Move();
    base.OnTurn();
  }

  private void Move() {
    List<Vector2Int> moveWaypoints = CalculateMoveWaypoints(XVelocity, YVelocity);
    for (int i = 1; i < moveWaypoints.Count; i++) {
      Vector2Int waypoint = moveWaypoints[i];
      int newRow = waypoint.y;
      int newCol = waypoint.x;

      //Is the new Tile a legal place to be?
      Tile below = GameManager.S.Board.GetInDirection(newRow, newCol, Direction.South);
      if (!CanStandAbove(below) || !CanSetPosition(newRow, newCol)) {
        XVelocity *= -1;
        break;
      }

      SetPosition(newRow, newCol, out bool success);
      if (!success) {
        throw new System.Exception("Unexpected failure in SetPosition");
      }
    }
  }

  private bool CanStandAbove(Tile tile) {
    foreach (ITileInhabitant inhabitant in tile.Inhabitants) {
      if (inhabitant is Platform) {
        return true;
      }
    }
    return false;
  }

  public static PlatformBeetle Make(PlatformBeetleObject platformBeetlePrefab, int row, int col, Transform parent = null) {
    platformBeetlePrefab = Object.Instantiate(platformBeetlePrefab);
    platformBeetlePrefab.transform.parent = parent;
    platformBeetlePrefab.spawnRow = row;
    platformBeetlePrefab.spawnCol = col;
    return new PlatformBeetle(platformBeetlePrefab);
  }
}
