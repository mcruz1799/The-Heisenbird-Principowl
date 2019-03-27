using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PopupManager : MonoBehaviour
{
    // [SerializeField] private GameObject player;
    [SerializeField] private RawImage[] popup1;

    //target is the player's x position where we want the popup to be initialized
    // [SerializeField] private float target1;
    // private bool target1Flag = false;
    [SerializeField] private RawImage[] popup2;
    // [SerializeField] private float target2;
    // private bool target2Flag = false;
    [SerializeField] private RawImage[] bossPopup;
    // [SerializeField] private float bossTarget;
    // private bool bossTargetFlag = false;
    private CameraFollow cf;
    private bool Level1Flag = false;
    private bool Level2Flag = false;
    private bool BossFlag = false;
    [SerializeField] private int[] rows = new int[1];
    [SerializeField] private int[] cols = new int[1];
    private Transform[] tileTransforms;
    [SerializeField] private float[] timesBetween;

    // private Tile tile = GameManager.S.Board[Row,Col]
    // Start is called before the first frame update
    private void Awake() {
        tileTransforms = new Transform[rows.Length];
        for (int i = 0; i < tileTransforms.Length; i++)
        {
            tileTransforms[i] = GetTransform(rows[i],cols[i]);
        }
        StartCoroutine(PanRoutine());

    }
    private IEnumerator PanRoutine() {
        string SceneName = SceneManager.GetActiveScene().name;
        char level = SceneName[SceneName.Length-1];
        bool bossCheck = (SceneName[SceneName.Length-2] == '_') ? true : false;
        cf.PanCamera(tileTransforms,timesBetween);
        if (level == '1' && !bossCheck)
        {
            StartCoroutine(PopupRoutine(popup1));
        }
        if (level == '2')
        {
            StartCoroutine(PopupRoutine(popup2));
        }
        if (level == '1' && bossCheck)
        {
            StartCoroutine(PopupRoutine(bossPopup));
        }
        yield return null;
    }

    private IEnumerator PopupRoutine(RawImage[] popups){
        GameManager.S.CurrentState = GameManager.GameState.Stopped; 

        for (int i = 0; i < tileTransforms.Length; i++) 
        {
            RawImage currPopup = popups[i];
            float currTime = timesBetween[i];

            cf.PanNext();
            currPopup.gameObject.SetActive(true);
            yield return new WaitForSeconds(currTime);
            currPopup.gameObject.SetActive(false);
        }
        cf.PanToPlayer(3.0f);
        cf.StopPanning();

        GameManager.S.CurrentState = GameManager.GameState.Running; 

    }

    private Transform GetTransform(int row, int col)
    {
        return GameManager.S.Board[row,col].transform;
    }
}
