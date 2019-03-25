using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private RawImage popup1;
    [SerializeField] private RawImage popup2;
    [SerializeField] private RawImage bossPopup;
    // Start is called before the first frame update
    
    private IEnumerator Level1Routine()
    {
        yield return null;
    }

    private IEnumerator Level2Routine()
    {
        yield return null;
    }
    
    private IEnumerator BossRoutine()
    {
        yield return null;
    }
}
