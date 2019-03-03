using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : Platform
{
  protected override int platformLength 
  {
    get { return 3; } //Replace with proper branch length.
    set { platformLength = value; }
  }
}
