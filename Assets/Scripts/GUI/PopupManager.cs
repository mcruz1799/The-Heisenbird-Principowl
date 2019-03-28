using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private new CameraFollow camera;
  [SerializeField] private PopupInfo[] popups;
  [SerializeField] private PopupInfo[] LocationPopups;
#pragma warning restore 0649

  private void Start() {
    StartCoroutine(PopupRoutine());
  }

  public IEnumerator PopupRoutine() {
    //Save current game manager state
    GameManager.GameState oldState = GameManager.S.CurrentState;

    //Pause the game, pan over targets
    GameManager.S.CurrentState = GameManager.GameState.Stopped;
    foreach (PopupInfo info in popups) {
      info.popup.gameObject.SetActive(true);
      camera.PanTo(GameManager.S.Board[info.xyCoords.y, info.xyCoords.x].transform);
      yield return new WaitForSeconds(info.timeToPause);
      info.popup.gameObject.SetActive(false);
    }

    camera.StopPanning();

    //Restore game manager state to what it was
    GameManager.S.CurrentState = oldState;
  }

  //Enables playing a popup when the user reaches a location.
  public IEnumerator PopupForLocation(int index)
  {
    PopupInfo popup = LocationPopups[index];
    popup.popup.gameObject.SetActive(true);
    yield return new WaitForSeconds(popup.timeToPause);
    popup.popup.gameObject.SetActive(false);
  }

  [System.Serializable]
  private struct PopupInfo {
#pragma warning disable 0649
    public RawImage popup;
    public Vector2Int xyCoords;
    public float timeToPause;
#pragma warning restore 0649
  }
}
