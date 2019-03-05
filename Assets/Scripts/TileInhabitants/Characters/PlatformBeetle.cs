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

  private int turnParity = 0;
  public override void OnTurn() {
    //It's safe to do this stuff before OnDeath() is check in the base method.
    //Awful stylistically, but safe.  Please don't add more code unless you really know what you're doing.
    turnParity += 1;
    turnParity %= e.moveCooldown;
    if (turnParity == 0) {
      Move();
    }
    base.OnTurn();
  }

  protected override void OnDeath() {
    Debug.Log("Beetle died!");
    PlatformToggleManager.Toggle(GroupColor);
    Object.Destroy(e.gameObject);
    GameManager.S.Board[Row, Col].Remove(this);
    GameManager.S.UnregisterTurnTaker(this);
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
