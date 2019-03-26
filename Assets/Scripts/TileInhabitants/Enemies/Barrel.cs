using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSubEntity : EnemySubEntity<Barrel, BarrelSubEntity> {
  private Direction AttackDirection => Direction.South;

  public BarrelSubEntity(SingleTileEntityObject gameObject, Barrel parent, out bool success) : base(gameObject, parent, out success) {
  }

  public override void Attack() {
    Tile t = GameManager.S.Board.GetInDirection(Row, Col, AttackDirection);
    if (t != null) {
      foreach (ITileInhabitant other in t.Inhabitants) {
        IDamageable victim = other is IDamageable ? (IDamageable)other : null;
        if (victim != null && CanAttack(other)) {
          victim.OnAttacked(parent.AttackPower, AttackDirection);
        }
      }
    }
  }

  public bool IsGrounded => CanStandAbove(GameManager.S.Board.GetInDirection(Row, Col, Direction.South));

  public bool WillBeAboveGroundAfterMoveInDirection(Direction direction) {
    Tile t;
    t = GameManager.S.Board.GetInDirections(Row, Col, direction, Direction.South);

    return CanStandAbove(t);
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

  private bool CanAttack(ITileInhabitant other) {
    return other is PlayerLabel && !toIgnore.Contains(other);
  }
}

public class Barrel : Enemy<Barrel, BarrelSubEntity> {
  private readonly BarrelObject gameObject;

  private Barrel(BarrelObject gameObject, out bool success) : base(gameObject, out success) {
    this.gameObject = gameObject;
  }


  public override void Destroy() {
    //TODO: replace with boss bonk sound
    SoundManager.S.BeetleDied();
    base.Destroy();
  }

  protected override void OnCollision(Direction moveDirection) {
    return;
  }

  public override void OnTurn(){
    //barrel only breaks on hitting ground, you, or colored platform
    //check platform ColorGroup -> if not None, destroy barrel
    if( /* some condition to throw barrel */ true ){
      //instantiate barrel
    }
  }

  public override void OnAttacked(int attackPower, Direction attackDirection){
    //TODO: do handling for final bonk/create final ending level
    GameManager.S.LoadNextLevel();
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