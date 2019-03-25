using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelUIData : MonoBehaviour
{
  [SerializeField] int levelIndex;
  [SerializeField] GameObject Score;
  [SerializeField] GameObject _Locked;
  private bool Locked {
    get {
      return _Locked.activeSelf;
    }
    set {
      _Locked.SetActive(value);
    }
  }

  void OnEnable()
  {
    Debug.Log("Filling level information...");
    OverallProgress save = BinarySaver.S.currentSave;
    LevelProgress level = save.levels[levelIndex];
    if (levelIndex > 0) {
      LevelProgress previousLevel = save.levels[levelIndex - 1];
      Locked = !previousLevel.completed; //If the previous level hasn't been completed, lock the level.
    }
    if (level.completed) {
      Text score = Score.GetComponent<Text>();
      score.text = level.score.ToString();
    }
  }

  public void playLevel(string s)
  {
    if (_Locked == null || !Locked) {
      SceneManager.LoadScene(s);
    } else { Debug.Log("Level is Locked.");  };
  }
}
