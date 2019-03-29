using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSubEntity : EnemySubEntity<Barrel, BarrelSubEntity> {
  private static readonly Direction[] attackDirections = new Direction[] { Direction.South, Direction.East, Direction.West };

  public BarrelSubEntity(SingleTileEntityObject gameObject, Barrel parent, out bool success) : base(gameObject, parent, out success) {
  }

  public override void Attack() {
    foreach (Direction d in attackDirections) {
      Tile t = GameManager.S.Board.GetInDirection(Row, Col, d);
      if (t != null) {
        foreach (ITileInhabitant other in t.Inhabitants) {
          IDamageable victim = other is IDamageable ? (IDamageable)other : null;
          if (victim != null && CanAttack(other)) {
            victim.OnAttacked(parent.AttackPower, d);
          }
        }
      }
    }
  }

  private bool CanAttack(ITileInhabitant other) {
    return other is IPlayer && !toIgnore.Contains(other);
  }
}

public class Barrel : Enemy<Barrel, BarrelSubEntity> {
  private readonly BarrelObject gameObject;

  private Barrel(BarrelObject gameObject, out bool success) : base(gameObject, out success) {
    this.gameObject = gameObject;
    YVelocity = -1;
    XVelocity = 0;
  }

  protected override void OnCollision(Direction moveDirection) {
    OnAttacked(int.MaxValue, moveDirection.Opposite());
  }

  protected override void OnTurnCore() {
    //Do nothing
  }

  private class SubEntityGameObject : SingleTileEntityObject {
    public Barrel parent;
    public override float MoveAnimationTime => parent.gameObject.MoveAnimationTime;
  }

  protected override BarrelSubEntity CreateSubEntity(EnemyObject e, int row, int col, out bool success) {
    SubEntityGameObject subentityGameObject = new GameObject().AddComponent<SubEntityGameObject>();
    subentityGameObject.parent = this;
    subentityGameObject.name = string.Format("Barrel[r={0}, c={1}]", row, col); ;
    subentityGameObject.spawnRow = row;
    subentityGameObject.spawnCol = col;
    subentityGameObject.transform.parent = e.transform;
    return new BarrelSubEntity(subentityGameObject, this, out success);
  }

  public static Barrel Make(BarrelObject barrelPrefab, int row, int col, Transform parent = null) {
    barrelPrefab = Object.Instantiate(barrelPrefab);
    barrelPrefab.transform.parent = parent;
    barrelPrefab.spawnRow = row;
    barrelPrefab.spawnCol = col;
    Barrel result = new Barrel(barrelPrefab, out bool success);
    return success ? result : null;
  }
}