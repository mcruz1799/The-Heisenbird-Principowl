using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    OverallProgress save = BinarySaver.S.currentSave;
    LevelProgress level = save.levels[levelIndex];
    if (levelIndex > 0) {
      LevelProgress previousLevel = save.levels[levelIndex - 1];
      Locked = !previousLevel.completed; //If the previous level hasn't been completed, lock the level.
    }
  }

  void playLevel(string s)
  {
    if (!Locked) {
      SceneManager.LoadScene(s);
    } else { Debug.Log("Level is Locked.");  };
  }
}
