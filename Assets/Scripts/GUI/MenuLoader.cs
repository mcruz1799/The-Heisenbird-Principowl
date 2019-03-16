using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoader : MonoBehaviour
{
  public void LoadScene(string s){
    SceneManager.LoadScene(s);
  }

  public void Quit(){
    Application.Quit();
  }
}
