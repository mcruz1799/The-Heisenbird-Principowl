﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlayerLabel { }

  public sealed class PlayerSubEntity : SingleTileEntity, IDamageable, PlayerLabel {
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

    public PlayerSubEntity(SingleTileEntityObject gameObject, Player parent, out bool success) : base(gameObject, out success) {
      if (success) {
        this.parent = parent;
        this.gameObject = gameObject;
      }
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

          //Ignore inactive platforms
          if (!platform.IsActive) {
            continue;
          }

          //If the player can jump through the platform, ignore it unless you're the bottom of the player and are above it
          if (platform.PlayerCanJumpThrough) {
            bool isBottom = parent.SubEntityIsBottom(this);
            bool isImmediatelyAbove = Row > platform.Row;
            if (!(isBottom && isImmediatelyAbove)) {
              continue;
            }
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
