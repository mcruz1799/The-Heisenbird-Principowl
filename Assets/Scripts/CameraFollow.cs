using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private Vector3 localOffset;
  [SerializeField] private bool useInspectorOffset;

  [SerializeField] private bool smoothMotion;
  [Range(1f, 100f)] [SerializeField] private float smoothMotionSpeed = 4f;
  [Range(1f, 100f)] [SerializeField] private float panningSpeed = 2f;

  [SerializeField] private bool lockX;
  [SerializeField] private bool lockY;
  [SerializeField] private bool lockZ;

  [SerializeField] private CustomCameraArea[] customCameraAreas;
#pragma warning restore 0649

  private float xThreshold = 0.1f;
  private float yThreshold = 0.1f;

  private float xDifference;
  private float yDifference;
  private Transform panTarget;


  private void Start() {
    if (!useInspectorOffset) {
      localOffset = transform.position - GameManager.S.Player.WorldPosition;
    }
  }

  private void LateUpdate() {
    //Once here, assumes that it is following the Player.
    Vector3 currentPosition = transform.position;
    Vector3 targetPosition;
    float speed = smoothMotionSpeed;
    if (panTarget != null) {
      targetPosition = panTarget.transform.position + localOffset;
      speed = panningSpeed;
    } else {
      targetPosition = GameManager.S.Player.WorldPosition + localOffset;
      foreach (CustomCameraArea area in customCameraAreas) {
        if (area.ContainsPlayer()) {
          targetPosition += area.additionalOffset;
          break;
        }
      }
    }

    Vector3 finalPosition;
    finalPosition = targetPosition;

    if (smoothMotion) {
      xDifference = Mathf.Abs(targetPosition.x - currentPosition.x);
      yDifference = Mathf.Abs(targetPosition.y - currentPosition.y);

      if (xDifference >= xThreshold || yDifference >= yThreshold) {
        finalPosition = Vector3.Lerp(currentPosition, targetPosition, speed * Time.deltaTime);
      }
      finalPosition.x = (xDifference < xThreshold) ? currentPosition.x : finalPosition.x;
      finalPosition.y = (yDifference < yThreshold) ? currentPosition.y : finalPosition.y;

      transform.position = finalPosition;
    }
  }

  public void PanTo(Transform transform) {
    panTarget = transform;
  }

  public void StopPanning() {
    panTarget = null;
  }


  [System.Serializable] private struct CustomCameraArea {
#pragma warning disable 0649
    [SerializeField] private RectInt areaBounds;
    public Vector3 additionalOffset;
#pragma warning restore 0649

    public bool ContainsPlayer() {
      return areaBounds.Contains(new Vector2Int(GameManager.S.Player.Col, GameManager.S.Player.Row));
    }
  }

}



