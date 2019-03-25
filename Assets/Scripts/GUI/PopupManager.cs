using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] [Range(1,120)] private float popupTime;
    [SerializeField] private RawImage popup1;
    //target is the player's x position where we want the popup to be initialized
    [SerializeField] private float target1;
    private bool target1Flag = false;
    [SerializeField] private RawImage popup2;
    [SerializeField] private float target2;
    private bool target2Flag = false;

    [SerializeField] private RawImage bossPopup;
    [SerializeField] private float bossTarget;
    private bool bossTargetFlag = false;

    // Start is called before the first frame update
    private void Update() {
        string SceneName = SceneManager.GetActiveScene().name;
        char level = SceneName[SceneName.Length-1];
        if (level == '1' && !target1Flag)
        {
            if (player.transform.position.x >= target1)
            {
                target1Flag = true;
                StartCoroutine(Level1Routine());
            }
        }
        if (level == '2' && !target2Flag)
        {
            if (player.transform.position.x >= target2)
            {
                target2Flag = true;
                StartCoroutine(Level2Routine());
            }
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
