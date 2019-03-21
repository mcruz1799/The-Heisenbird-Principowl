using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerLabel { }

public partial class Player {
  private sealed class PlayerSubEntity : SingleTileEntity, IDamageable, PlayerLabel {
    private readonly Player parent;

    public bool IsGrounded {
      get {
        //Can't fall out of bounds
        if (!GameManager.S.Board.IsPositionLegal(Row - 1, Col)) {
          return true;
        }

        //If you can't be added to the Tile, it counts as ground.
        if (!GameManager.S.Board[Row - 1, Col].CanAdd(this)) {
          return true;
        }

        return false;
      }
    }
    public bool InUpdraft {
      get {
        foreach (var inhabitant in GameManager.S.Board[Row, Col].Inhabitants) {
          if (inhabitant is UpdraftTile) {
            return true;
          }
        }
        return false;
      }
    }

    public SingleTileEntityObject gameObject;

    public PlayerSubEntity(SingleTileEntityObject gameObject, Player parent) : base(gameObject) {
      this.parent = parent;
      this.gameObject = gameObject;
    }

    public new List<Vector2Int> CalculateMoveWaypoints(int xDelta, int yDelta) {
      return base.CalculateMoveWaypoints(xDelta, yDelta);
    }


    //
    //SingleTileEntity
    //

    protected override bool IsBlockedByCore(ITileInhabitant other) {
      if (other is Enemy) {
        return true;
      }

      if (other is Platform) {
        Platform platform = (Platform)other;
        if (!platform.IsActive) {
          return false;
        }
        if (platform.PlayerCanJumpThrough && platform.Row == Row + 1) {
          return false;
        }
        if (parent.IsDroppingThroughPlatform && platform.PlayerCanDropThrough && platform.Row == Row - 1) {
          return false;
        }
        return true;
      }

      return false;
    }


    //
    //IDamageable
    //

    public int MaxHitpoints => parent.MaxHitpoints;
    public int Hitpoints => parent.Hitpoints;
    public bool IsAlive => parent.IsAlive;

    public int CalculateDamage(int baseDamage) {
      return parent.CalculateDamage(baseDamage);
    }

    public void OnAttacked(int attackPower, Direction attackDirection) {
      parent.OnAttacked(attackPower, attackDirection);
    }

    public override void Destroy() {
      base.Destroy();
    }
  }
}