using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : SingleTileEntity, ITurnTaker, IAttacker, IDamageable {
#pragma warning disable 0649
  [Range(1, 1000)] [SerializeField] private int _maxHp = 1;
  [Range(1, 1000)] [SerializeField] private int _attackPower = 1;
#pragma warning restore 0649

  protected Direction Facing { get; set; }

  protected virtual void Awake() {
    GameManager.S.RegisterTurnTaker(this);
    _damageable = new Damageable(_maxHp);
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

    other.TakeDamage(this, _attackPower);
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
