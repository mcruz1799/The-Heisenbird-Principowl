using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private Transform toFollow;
  [SerializeField] private Vector3 localOffset;
  [SerializeField] private bool useInspectorOffset;

  [SerializeField] private bool smoothMotion;
  [Range(0.01f, 100f)] [SerializeField] private float smoothMotionSpeed = 1f;

  [SerializeField] private bool lockX;
  [SerializeField] private bool lockY;
  [SerializeField] private bool lockZ;
#pragma warning restore 0649

  private void Awake() {
    if (!useInspectorOffset) {
      localOffset = transform.position - toFollow.position;
    }
  }

  private void Update() {
    Vector3 currentPosition = transform.position;
    Vector3 targetPosition = toFollow.position + localOffset;

    Vector3 finalPosition;
    if (smoothMotion) {
      finalPosition = Vector3.Lerp(currentPosition, targetPosition, smoothMotionSpeed * Time.deltaTime);
    } else {
      finalPosition = targetPosition;
    }

    finalPosition.x = lockX ? currentPosition.x : finalPosition.x;
    finalPosition.y = lockY ? currentPosition.y : finalPosition.y;
    finalPosition.z = lockZ ? currentPosition.z : finalPosition.z;
    transform.position = finalPosition;
  }
}