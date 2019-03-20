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

  private int turnParity;
  public override void OnTurn() {
    //It's safe to do this stuff before OnDeath() is check in the base method.
    //Awful stylistically, but safe.  Please don't add more code unless you really know what you're doing.
    if (e.moveCooldown != 0) {
      turnParity += 1;
      turnParity %= e.moveCooldown;
    }
    Move();
    base.OnTurn();
  }

  protected override void OnDeath() {
    SoundManager.S.BeetleDied();
    PlatformToggleManager.Toggle(GroupColor);
    Destroy();
  }

  private void Move() {
    if (!CanStandAbove(GameManager.S.Board.GetInDirection(Row, Col, Direction.South))) {
      //Seems like the ground disappeared from beneath me!  D:
      Fall();

    } else {
      //Business as usual
      if (turnParity == 0) {
        Patrol();
      }
    }
  }

  private void Fall() {
    YVelocity -= 1;

    List<Vector2Int> moveWaypoints = CalculateMoveWaypoints(0, YVelocity);
    for (int i = 1; i < moveWaypoints.Count; i++) {
      Vector2Int waypoint = moveWaypoints[i];
      int newRow = waypoint.y;
      int newCol = waypoint.x;

      if (!CanSetPosition(newRow, newCol)) {
        YVelocity = 0;
        break;
      }

      SetPosition(newRow, newCol, out bool success);
      if (!success) {
        throw new System.Exception("Unexpected failure in SetPosition");
      }
    }
  }

  private void Patrol() {
    List<Vector2Int> moveWaypoints = CalculateMoveWaypoints(XVelocity, 0);
    for (int i = 1; i < moveWaypoints.Count; i++) {
      Vector2Int waypoint = moveWaypoints[i];
      int newRow = waypoint.y;
      int newCol = waypoint.x;

      //Unless I'm falling, my next waypoint must be on the ground.
      Tile below = GameManager.S.Board.GetInDirection(newRow, newCol, Direction.South);
      if (!CanSetPosition(newRow, newCol) || !CanStandAbove(below)) {
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
    if (tile == null) {
      return true;
    }
    foreach (ITileInhabitant inhabitant in tile.Inhabitants) {
      if (inhabitant is Platform) {
        Platform platform = (Platform)inhabitant;
        return platform.IsActive;
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
