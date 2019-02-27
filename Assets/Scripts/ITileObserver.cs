using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileObserver {
  void OnInhabitantEntered(ITileInhabitant entered);
  void OnInhabitantExited(ITileInhabitant exited);
}