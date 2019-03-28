using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompletionDisplay : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private GameObject panel;
#pragma warning restore 0649

  private void Update() {
    if (GameManager.S.CurrentState == GameManager.GameState.GameOver) {
      panel.SetActive(true);
    }
  }

  public void LoadNextLevel() {
    GameManager.S.LoadNextLevel();
  }

  public void LoadMenu() {
    GameManager.S.LoadMenu();
  }
}
