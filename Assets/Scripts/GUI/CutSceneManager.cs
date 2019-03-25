using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        GameManager.S.GameState = "Stopped";
        foreach(RawImage slide in Slides)
        {
            slide.gameObject.SetActive(true);
            if (Input.anyKey) 
                slide.gameObject.SetActive(false);
                continue;
        }
        //start the game here
        yield return null;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
