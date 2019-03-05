using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : SingleTileEntity, ITurnTaker, IAttacker, IDamageable {

  private readonly EnemyObject e;
  protected Direction Facing { get; set; }
  protected int SpeedX { get; set; }
  protected int SpeedY { get; set; }

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
    Move();
    Tile attackedTile = GameManager.S.Board.GetInDirection(Row, Col, Facing);
    attackedTile.Attack(this);
  }

  public override bool CanSetPosition(int newRow, int newCol)
  {
    bool validPosition = base.CanSetPosition(newRow, newCol);
    Tile below = GameManager.S.Board.GetInDirection(newRow, newCol, Direction.South);
    
    return validPosition && !below.CanAdd(this); //Makes sure that there is something beneath the enemy.
  }
  private void Move()
  {
    List<Vector2Int> moveWaypoints = CalculateMoveWaypoints(SpeedX, SpeedY);

    for (int i = 1; i < moveWaypoints.Count; i++) {
      Vector2Int waypoint = moveWaypoints[i];
      int newRow = waypoint.y;
      int newCol = waypoint.x;

      SetPosition(newRow, newCol, out bool enteredNewPosition);

      if (!enteredNewPosition) {
        Facing = Facing.Opposite();
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
