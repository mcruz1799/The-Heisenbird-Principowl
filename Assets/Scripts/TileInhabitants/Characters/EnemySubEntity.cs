using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy {
  bool IsAlive { get; }
}

public abstract class EnemySubEntity<TParent, TSub> : SingleTileEntity, IDamageable, IEnemy
  where TParent : Enemy<TParent, TSub>
  where TSub : EnemySubEntity<TParent, TSub> {

  protected readonly TParent parent;
  public SingleTileEntityObject gameObject;

  public bool IsAlive => parent.IsAlive;

  public EnemySubEntity(SingleTileEntityObject gameObject, TParent parent) : base(gameObject) {
    this.gameObject = gameObject;
    this.parent = parent;
  }

  protected override bool IsBlockedByCore(ITileInhabitant other) {
    if (other is PlayerLabel || other is IEnemy) {
      return true;
    }

    if (other is Platform) {
      Platform platform = (Platform)other;
      return platform.IsActive;
    }

    return false;
  }

  public abstract void Attack();


  //
  //IDamageable
  //

  public virtual void OnAttacked(int attackPower, Direction attackDirection) {
    parent.OnAttacked(attackPower, attackDirection);
  }
}