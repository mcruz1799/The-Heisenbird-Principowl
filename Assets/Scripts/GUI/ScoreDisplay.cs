 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour {
  private int timeRemaining;
  private Text timerText;

  private void Awake() {
    timerText = GetComponent<Text>();
  }

  private void Update() {
    timerText.text = "Score: " + GameManager.S.Score.ToString();
  }
}
