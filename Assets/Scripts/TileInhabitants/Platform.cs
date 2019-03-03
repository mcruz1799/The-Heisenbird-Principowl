﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Platform : MultiTileEntity
{
  private Board Board => GameManager.S.Board;

  protected abstract int platformLength { get; set; }

  protected string color = "";

  protected override HashSet<SingleTileEntity> ConstructSelf()
  {
    HashSet<SingleTileEntity> self = new HashSet<SingleTileEntity>();

    for (int i = 0; i < platformLength; i++) {
      Wall w = new Wall();
      self.Add(w);
    }

    return self;
  }

}
