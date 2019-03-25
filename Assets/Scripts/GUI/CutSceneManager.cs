using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{   
    [SerializeField] [Range(1,10)] private int slideNum;
    [SerializeField] private RawImage[] Slides;
    // Start is called before the first frame update
    void Start()
    {
        Slides = new RawImage[slideNum];
    }

    private IEnumerator CutSceneRoutine()
    {
        GameManager.S.CurrentState = GameManager.GameState.Stopped;
        foreach(RawImage slide in Slides)
        {
            slide.gameObject.SetActive(true);
            if (Input.anyKey) 
                slide.gameObject.SetActive(false);
                continue;
        }
        //start the game here
        GameManager.S.CurrentState = GameManager.GameState.Running;
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        //start the cutscene routine when appropriate
    }
}
