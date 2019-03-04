using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MultiTileEntity : MonoBehaviour, ITileInhabitant {
#pragma warning disable 0649
#pragma warning restore 0649

  //The abstract class gets to define what composedEntities actually contains
  //Subclasses aren't allowed to modify what's inside composedEntities, but they can look via ComposedEntities
  private readonly Dictionary<SingleTileEntity,Vector2Int> composedEntities = new Dictionary<SingleTileEntity, Vector2Int>();
  //protected IReadOnlyCollection<SingleTileEntity> ComposedEntities => composedEntities;

  protected abstract System.Tuple<Dictionary<SingleTileEntity,Vector2Int>, SingleTileEntity> ConstructSelf();

  private SingleTileEntity _leadingEntity;
  protected SingleTileEntity LeadingEntity {
    get => _leadingEntity;
    set => _leadingEntity = 
      composedEntities.ContainsKey(value) ? 
      _leadingEntity = value : 
      throw new System.ArgumentException("LeadingEntity must be a member of composedEntities");
  }

  protected virtual void Awake()
  {
    System.Tuple<Dictionary<SingleTileEntity,Vector2Int>, SingleTileEntity > tuple = ConstructSelf();
    foreach (SingleTileEntity entity in tuple.Item1.Keys) {
      composedEntities.Add(entity, tuple.Item1[entity]);
    }
    LeadingEntity = (tuple.Item2 != null) ? tuple.Item2 : throw new System.ArgumentException("LeadingEntity is null.");
  }

  public void SetPosition(int newRow, int newCol, out bool success) {
    foreach (SingleTileEntity entity in composedEntities.Keys) {
      int row = newRow + composedEntities[entity].y;
      int col = newCol + composedEntities[entity].x;
      if (!entity.CanSetPosition(row, col)) {
        success = false;
        return;
      }
    }

    success = true;
    foreach (SingleTileEntity entity in composedEntities.Keys) {


      int row = newRow + composedEntities[entity].y;
      int col = newCol + composedEntities[entity].x;
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
    foreach (SingleTileEntity entity in composedEntities.Keys) {
      bool otherIsPartOfThis = other is SingleTileEntity && composedEntities.ContainsKey((SingleTileEntity)other);
      if (!otherIsPartOfThis && entity.IsBlockedBy(other)) {
        return true;
      }
    }
    return false;
  }

  public ISet<Tile> Occupies() {
    ISet<Tile> result = new HashSet<Tile>();
    foreach (SingleTileEntity entity in composedEntities.Keys) {
      result.UnionWith(entity.Occupies());
    }
    return result;
  }
}
