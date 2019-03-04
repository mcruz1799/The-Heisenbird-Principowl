using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Platform : MultiTileEntity
{
  private Board Board => GameManager.S.Board;

  protected abstract int platformLength { get; set; }

  protected abstract string Color { get; }
}
