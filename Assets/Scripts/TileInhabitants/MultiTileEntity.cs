using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MultiTileEntity : ITileInhabitant {

  //The abstract class gets to define what composedEntities actually contains
  //Subclasses aren't allowed to modify what's inside composedEntities, but they can look via ComposedEntities
  private readonly HashSet<SingleTileEntity> composedEntities = new HashSet<SingleTileEntity>();
  protected IReadOnlyCollection<SingleTileEntity> ComposedEntities => composedEntities;

  private SingleTileEntity _leadingEntity;
  protected SingleTileEntity LeadingEntity {
    get => _leadingEntity;
    set => _leadingEntity = 
      composedEntities.Contains(value) ? 
      _leadingEntity = value : 
      throw new System.ArgumentException("LeadingEntity must be a member of composedEntities");
  }

  protected MultiTileEntity(HashSet<SingleTileEntity> composedEntities, SingleTileEntity leadingEntity) {
    this.composedEntities.UnionWith(composedEntities);
    LeadingEntity = leadingEntity;
  }

  public void SetPosition(int newRow, int newCol, out bool success) {
    foreach (SingleTileEntity entity in composedEntities) {
      int row = newRow + (entity.Row - LeadingEntity.Row);
      int col = newCol + (entity.Col - LeadingEntity.Col);
      if (!entity.CanSetPosition(row, col)) {
        success = false;
        return;
      }
    }

    success = true;
    foreach (SingleTileEntity entity in composedEntities) {


      int row = newRow + (entity.Row - LeadingEntity.Row);
      int col = newCol + (entity.Col - LeadingEntity.Col);
      Debug.Log("Multi Row:" + row);
      Debug.Log("Multi Col:" + col);
      entity.SetPosition(row, col, out bool doubleCheckSuccess);
      if (!doubleCheckSuccess) {
        success = false;
        Debug.LogError("Unexpected failure in SetPosition");
        throw new System.InvalidOperationException("Unexpected failure in SetPosition");
      }
    }

  }


  //
  //ITileInhabitant
  //

  public bool IsBlockedBy(ITileInhabitant other) {
    foreach (SingleTileEntity entity in composedEntities) {
      bool otherIsPartOfThis = other is SingleTileEntity && composedEntities.Contains((SingleTileEntity)other);
      if (!otherIsPartOfThis && entity.IsBlockedBy(other)) {
        return true;
      }
    }
    return false;
  }

  public ISet<Tile> Occupies() {
    ISet<Tile> result = new HashSet<Tile>();
    foreach (SingleTileEntity entity in composedEntities) {
      result.UnionWith(entity.Occupies());
    }
    return result;
  }
}
