﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
#pragma warning disable 0649
  [SerializeField] private Transform toFollow;
  [SerializeField] private Vector3 localOffset;
  [SerializeField] private bool useInspectorOffset;

  [SerializeField] private bool smoothMotion;
  [Range(1f, 100f)] [SerializeField] private float smoothMotionSpeed = 4f;

  [SerializeField] private float xThreshold = 0;
  [SerializeField] private float yThreshold = 3;

  [SerializeField] private bool lockX;
  [SerializeField] private bool lockY;
  [SerializeField] private bool lockZ;
#pragma warning restore 0649

  private float xDifference;
  private float yDifference;
  private Vector3 moveTemp;

  private void Awake() {
    if (!useInspectorOffset) {
      localOffset = transform.position - toFollow.position;
    }
  }

  private void LateUpdate() {
    Vector3 currentPosition = transform.position;
    Vector3 targetPosition = toFollow.position + localOffset;

    Vector3 finalPosition;
    finalPosition = targetPosition;

    if (smoothMotion) {
      xDifference = Mathf.Abs(targetPosition.x - currentPosition.x);
      yDifference = Mathf.Abs(targetPosition.y - currentPosition.y);

      if (xDifference >= xThreshold || yDifference >= yThreshold) {
        finalPosition = Vector3.Lerp(currentPosition, targetPosition, smoothMotionSpeed * Time.deltaTime);
      }
      finalPosition.x = (xDifference < xThreshold) ? currentPosition.x : finalPosition.x;
      finalPosition.y = (yDifference < yThreshold) ? currentPosition.y : finalPosition.y;

      transform.position = finalPosition;
    } else {
      finalPosition = targetPosition;
    }
  }
}
