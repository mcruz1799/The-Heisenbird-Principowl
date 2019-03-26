﻿using System.Collections;
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
          Destroy();
        }
      }
    }
  }

  private bool CanAttack(ITileInhabitant other) {
    return other is PlayerLabel && !toIgnore.Contains(other);
  }
}

public class Barrel : Enemy<Barrel, BarrelSubEntity> {
  private readonly BarrelObject gameObject;

  private Barrel(BarrelObject gameObject, out bool success) : base(gameObject, out success) {
    this.gameObject = gameObject;
    this.YVelocity = -1;
  }


  public override void Destroy() {
    SoundManager.S.PlayerDamaged();
    base.Destroy();
  }

  protected override void OnCollision(Direction moveDirection) {}

  public override void OnTurn(){  
    //First attack, then move
    //Destroy once we reach the ground
    if (this.TopLeft.Row <= 2) Destroy();

    //First attempt to attack
    TopLeft.Attack();

    //Then check if anything is beneath us
    Tile t = GameManager.S.Board.GetInDirection(TopLeft.Row, TopLeft.Col, Direction.South);
    if (t) {
      foreach (ITileInhabitant item in t.Inhabitants){
        if (item is Platform){
          Platform platform = (Platform) item;

          //If we are above a colored platform, kamikaze
          if (platform.IsActive && platform.ColorGroup != PlatformAndBeetleColor.None){
              Destroy();
          }
        }
      }
    }

    //Nothing below us, we are clear for takeoff
    YVelocity = -1;
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