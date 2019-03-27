using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour
{
    // [SerializeField] private GameObject player;
    [SerializeField] [Range(1,120)] private float popupTime;
    [SerializeField] private RawImage popup1;

    //target is the player's x position where we want the popup to be initialized
    // [SerializeField] private float target1;
    // private bool target1Flag = false;
    [SerializeField] private RawImage popup2;
    // [SerializeField] private float target2;
    // private bool target2Flag = false;

    [SerializeField] private RawImage bossPopup;
    // [SerializeField] private float bossTarget;
    // private bool bossTargetFlag = false;

    private bool Level1Flag = false;
    private bool Level2Flag = false;
    private bool BossFlag = false;
    // Start is called before the first frame update
    private void Update() {
        string SceneName = SceneManager.GetActiveScene().name;
        char level = SceneName[SceneName.Length-1];
        bool bossCheck = (SceneName[SceneName.Length-2] == '_') ? true : false;
        if (level == '1' && !bossCheck && !Level1Flag)
        {
            Level1Flag = true;
            StartCoroutine(Level1Routine());
        }
        if (level == '2' && !bossCheck && !Level2Flag)
        {
            Level2Flag = true;
            StartCoroutine(Level2Routine());
        }
        if (level == '1' && bossCheck && !BossFlag)
        {
            BossFlag = true;
            StartCoroutine(BossRoutine());
        }
    }
    private IEnumerator Level1Routine()
    {
        popup1.gameObject.SetActive(true);
        yield return new WaitForSeconds(popupTime);
        popup1.gameObject.SetActive(false);
    }

    
    private IEnumerator Level2Routine()
    {
        popup2.gameObject.SetActive(true);
        yield return new WaitForSeconds(popupTime);
        popup2.gameObject.SetActive(false);
    }
    
    
    private IEnumerator BossRoutine()
    {
        bossPopup.gameObject.SetActive(true);
        yield return new WaitForSeconds(popupTime);
        bossPopup.gameObject.SetActive(false);
    }
}
