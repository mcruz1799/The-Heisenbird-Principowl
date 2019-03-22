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
        if (!CanSetPosition(Row - 1, Col)) {
          return true;
        }
        
        return false;
      }
    }
    public bool InUpdraft {
      get {
        foreach (ITileInhabitant inhabitant in GameManager.S.Board[Row, Col].Inhabitants) {
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


    //
    //SingleTileEntity
    //

    public override bool CanSetPosition(int newRow, int newCol) {
      if (!base.CanSetPosition(newRow, newCol)) {
        return false;
      }

      foreach (ITileInhabitant other in GameManager.S.Board[newRow, newCol].Inhabitants) {
        if (toIgnore.Contains(other)) {
          continue;
        }

        if (other is IEnemy) {
          return false;
        }

        if (other is Platform) {
          Platform platform = (Platform)other;
          if (!platform.IsActive) {
            continue;
          }
          if (platform.PlayerCanJumpThrough && platform.Row == Row + 1) {
            continue;
          }
          if (parent.IsDroppingThroughPlatform && platform.PlayerCanDropThrough && platform.Row == Row - 1) {
            Debug.LogWarning("Old code, may be incorrect");
            continue;
          }
          return false;
        }
      }

      return true;
    }


    //
    //IDamageable
    //

    public void OnAttacked(int attackPower, Direction attackDirection) {
      parent.OnAttacked(attackPower, attackDirection);
    }
  }
}