using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] [Range(1,120)] private float popupTime;
    [SerializeField] private RawImage popup1;
    //target is the player's x position where we want the popup to be initialized
    [SerializeField] private float target1;
    [SerializeField] private RawImage popup2;
    [SerializeField] private float target2;

    [SerializeField] private RawImage bossPopup;
    [SerializeField] private float targetBoss;

    // Start is called before the first frame update
    private void Update() {
        
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
