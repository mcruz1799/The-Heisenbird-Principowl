﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompletionDisplay : MonoBehaviour {
  private void Update() {
    if (GameManager.S.CurrentState == GameManager.GameState.GameOver) {
      gameObject.SetActive(true);
    }
  }

  public void LoadNextLevel() {
    GameManager.S.LoadNextLevel();
  }

  public void LoadMenu() {
    GameManager.S.LoadMenu();
  }
}
