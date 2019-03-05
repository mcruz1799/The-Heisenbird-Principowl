using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : SingleTileEntity, ITurnTaker, IAttacker, IDamageable {

  private readonly EnemyObject e;
  protected abstract Direction AttackDirection { get; }

  private int _xVelocity;
  protected int XVelocity {
    get => _xVelocity;
    set => _xVelocity = Mathf.Clamp(value, -e.xSpeedMax, e.xSpeedMax);
  }

  private int _yVelocity;
  protected int YVelocity {
    get => _yVelocity;
    set => _yVelocity = Mathf.Clamp(value, e.ySpeedMin, e.ySpeedMax);
  }

  public Enemy(EnemyObject e) : base(e) {
    this.e = e;
    GameManager.S.RegisterTurnTaker(this);
    _damageable = new Damageable(e._maxHp);
    Debug.LogWarning("Enemies have no cooldown on their attack, so they will damage the player every timestep");
  }

  protected override bool IsBlockedByCore(ITileInhabitant other) {
    return other is Platform || other is Player || other is Enemy;
  }

  public virtual void OnTurn() {
    //TODO: Shouldn't attack every timestep.  ^.-
    Tile attackedTile = GameManager.S.Board.GetInDirection(Row, Col, AttackDirection);
    foreach (ITileInhabitant inhabitant in attackedTile.Inhabitants) {
      if (!(inhabitant is IDamageable)) {
        continue;
      }

      IDamageable victim = (IDamageable)inhabitant;
      if (CanAttack(victim)) {
        victim.TakeDamage(this, e._attackPower);
      }
    }
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
