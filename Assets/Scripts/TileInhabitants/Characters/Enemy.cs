using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : SingleTileEntity, ITurnTaker, IDamageable {

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
  }

  protected override bool IsBlockedByCore(ITileInhabitant other) {
    if (other is Player.PlayerSubEntity || other is Enemy) {
      return true;
    }

    if (other is Platform) {
      Platform platform = (Platform)other;
      return platform.IsActive;
    }

    return false;
  }

  public virtual void OnTurn() {
    if (!IsAlive) {
      OnDeath();
      return;
    }

    //TODO: Shouldn't attack every timestep.  ^.-
    Tile attackedTile = GameManager.S.Board.GetInDirection(Row, Col, AttackDirection);
    if (attackedTile != null) {
      foreach (ITileInhabitant inhabitant in attackedTile.Inhabitants) {
        if (!(inhabitant is IDamageable)) {
          continue;
        }

        IDamageable victim = (IDamageable)inhabitant;
        if (CanAttack(victim)) {
          Attack(victim);
        }
      }
    }
  }


  //
  //IDamageable
  //

  private Damageable _damageable;
  public int MaxHitpoints => _damageable.MaxHitpoints;
  public int Hitpoints => _damageable.Hitpoints;
  public bool IsAlive => _damageable.IsAlive;

  public int CalculateDamage(int baseDamage) {
    return _damageable.CalculateDamage(baseDamage);
  }

  public virtual void TakeDamage(int baseDamage) {
    _damageable.TakeDamage(CalculateDamage(baseDamage));
  }

  protected abstract void OnDeath();

  //Enemies can only attack the player, not each other
  private bool CanAttack(IDamageable other) {
    return other is Player.PlayerSubEntity;
  }

  private void Attack(IDamageable other) {
    if (!CanAttack(other)) {
      Debug.LogError("Attempting an illegal attack");
      return;
    }

    other.TakeDamage(e._attackPower);
  }
}
