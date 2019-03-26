using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveUIData : MonoBehaviour
{
  [SerializeField] GameObject SaveName;
  [SerializeField] GameObject LevelComplete;

  public OverallProgress save;


  void OnEnable()
  {
    Debug.Log("Filling save information...");
    Text saveText = SaveName.GetComponent<Text>();
    saveText.text = save.saveName;
    Text completionText = LevelComplete.GetComponent<Text>();
    int i = 0;
    foreach (LevelProgress level in save.levels) {
      if (level.completed) i++;
    }
    completionText.text = $"{i}/3";
  }

  public void LoadSave()
  {
    BinarySaver.S.Load(save.saveName);
  }

}
