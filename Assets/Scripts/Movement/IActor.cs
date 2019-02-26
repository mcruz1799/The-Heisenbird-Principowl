using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActor {
  bool CanSelectAction(Action action);
  void SelectAction(Action action);
}