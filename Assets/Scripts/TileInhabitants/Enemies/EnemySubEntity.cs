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

  public EnemySubEntity(SingleTileEntityObject gameObject, TParent parent, out bool success) : base(gameObject, out success) {
    if (success) {
      this.gameObject = gameObject;
      this.parent = parent;
    }
  }

  public override bool CanSetPosition(int newRow, int newCol) {
    if (!base.CanSetPosition(newRow, newCol)) {
      return false;
    }

    foreach (ITileInhabitant other in GameManager.S.Board[newRow, newCol].Inhabitants) {
      if (toIgnore.Contains(other)) {
        continue;
      }

      if (other is PlayerLabel || other is IEnemy) {
        return false;
      }

      if (other is Platform) {
        Platform platform = (Platform)other;
        return !platform.IsActive;
      }
    }

    return true;
  }

  public abstract void Attack();


  //
  //IDamageable
  //

  public virtual void OnAttacked(int attackPower, Direction attackDirection) {
    parent.OnAttacked(attackPower, attackDirection);
  }
}