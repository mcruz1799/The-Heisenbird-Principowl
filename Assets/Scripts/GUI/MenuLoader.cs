using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLoader : MonoBehaviour
{
  [SerializeField] GameObject MainMenuGUI;
  [SerializeField] GameObject LevelSelectGUI;
  [SerializeField] GameObject SelectSaveGUI;
  [SerializeField] VerticalLayoutGroup saveLayoutGroup;
  [SerializeField] GameObject SavePrefab;

  public void LoadScene(string s){
    SceneManager.LoadScene(s);
  }

  public void Quit(){
    Application.Quit();
  }

  public void LoadLevels()
  {

    MainMenuGUI.SetActive(false);
    SelectSaveGUI.SetActive(false);
    LevelSelectGUI.SetActive(true);
  }

  public void LoadSaves()
  {
    saveLayoutGroup.transform.Clear();
    OverallProgress[] saves = BinarySaver.S.LoadSaves();
    foreach (OverallProgress save in saves) {
      SavePrefab.GetComponent<SaveUIData>().save = save;
      GameObject currentSave = GameObject.Instantiate(SavePrefab);
      currentSave.transform.SetParent(saveLayoutGroup.transform, false);
      currentSave.transform.SetAsLastSibling();
    }
    MainMenuGUI.SetActive(false);
    LevelSelectGUI.SetActive(false);
    SelectSaveGUI.SetActive(true);
  }

  public void LoadMenu()
  {
    MainMenuGUI.SetActive(true);
    SelectSaveGUI.SetActive(false);
    LevelSelectGUI.SetActive(false);
  }


}

public static class TransformEx
{
  public static Transform Clear(this Transform transform)
  {
    foreach (Transform child in transform) {
      GameObject.Destroy(child.gameObject);
    }
    return transform;
  }
}
