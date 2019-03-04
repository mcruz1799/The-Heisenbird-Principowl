using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : SingleTileEntity, ITurnTaker, IAttacker, IDamageable {

  private readonly EnemyObject e;
  protected Direction Facing { get; set; }

  public Enemy(EnemyObject e) : base(e) {
    this.e = e;
    GameManager.S.RegisterTurnTaker(this);
    _damageable = new Damageable(e._maxHp);
    Debug.LogWarning("Enemies have no cooldown on their attack, so they will damage the player every timestep");
  }

  public override bool IsBlockedBy(ITileInhabitant other) {
    bool isWall = other is Wall;
    return isWall;
  }

  public virtual void OnTurn() {
    //TODO: Shouldn't attack every timestep.  ^.-
    Tile attackedTile = GameManager.S.Board.GetInDirection(Row, Col, Facing);
    attackedTile.Attack(this);
  }


  //
  //IAttacker
  //

  //Enemies can only attack the player, not each other
  public bool CanAttack(IDamageable other) {
    return other is Player;
  }

  public void Attack(IDamageable other) {
    if (!CanAttack(other)) {
      Debug.LogError("Attempting an illegal attack");
      return;
    }

    other.TakeDamage(this, e._attackPower);
  }

  //
  //IDamageable
  //

  private Damageable _damageable;
  public int MaxHitpoints => _damageable.MaxHitpoints;
  public int Hitpoints => _damageable.Hitpoints;
  public bool IsAlive => _damageable.IsAlive;

  public int CalculateDamage(IAttacker attacker, int baseDamage) {
    return _damageable.CalculateDamage(baseDamage);
  }

  public void TakeDamage(IAttacker attacker, int baseDamage) {
    _damageable.TakeDamage(CalculateDamage(attacker, baseDamage));
    if (!IsAlive) {
      Debug.Log("TODO: Handle what happens when an enemy dies");
    }
  }
}
