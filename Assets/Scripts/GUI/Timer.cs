 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
  [Range(1, 999)] [SerializeField] private int _timerLength = 60;

  private int timeRemaining;
  private Text timerText;

  private void Awake() {
    timeRemaining = _timerLength;
    timerText = GetComponent<Text>();
    StartCoroutine(TimerRoutine());
  }

  private IEnumerator TimerRoutine() {
    while (timeRemaining > 0) {
      yield return new WaitForSeconds(1f);
      timeRemaining -= 1;
      timerText.text = timeRemaining.ToString();
    }
  }

  public int currentTimeLeft()
  {
    return timeRemaining;
  }
}
