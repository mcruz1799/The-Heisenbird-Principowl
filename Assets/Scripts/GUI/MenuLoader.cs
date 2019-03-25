using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoader : MonoBehaviour
{
  [SerializeField] GameObject MainMenuGUI;
  [SerializeField] GameObject LevelSelectGUI;

  public void LoadScene(string s){
    SceneManager.LoadScene(s);
  }

  public void Quit(){
    Application.Quit();
  }

  public void LoadLevels()
  {

    MainMenuGUI.SetActive(false);
    LevelSelectGUI.SetActive(true);
  }

  public void LoadMenu()
  {
    MainMenuGUI.SetActive(true);
    LevelSelectGUI.SetActive(false);
  }
}
