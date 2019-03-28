using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPositionDisplay : MonoBehaviour {
  private Text text;
  private void Awake() {
    text = GetComponent<Text>();
  }

  private void Update() {
    text.text = string.Format("(x, y) = ({0}, {1})", GameManager.S.Player.Col, GameManager.S.Player.Row);
  }
}
