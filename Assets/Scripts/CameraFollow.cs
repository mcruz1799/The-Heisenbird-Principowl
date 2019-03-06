using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
#pragma warning disable 0649
  [SerializeField] private Transform toFollow;
  [SerializeField] private Vector3 localOffset;
  [SerializeField] private bool useInspectorOffset;

  [SerializeField] private bool smoothMotion;
  [Range(1f, 100f)] [SerializeField] private float smoothMotionSpeed = 4f;

  [SerializeField] float xThreshold;
  [SerializeField] float yThreshold;

  [SerializeField] private bool lockX;
  [SerializeField] private bool lockY;
  [SerializeField] private bool lockZ;
#pragma warning restore 0649

  private float xDifference;
  private float yDifference;
  private Vector3 moveTemp;

  private void Awake()
  {
    if (!useInspectorOffset) {
      localOffset = transform.position - toFollow.position;
    }
    xThreshold = 0;
    yThreshold = 3;
  }

  private void Update()
  {
    Vector3 currentPosition = transform.position;
    Vector3 targetPosition = toFollow.position + localOffset;

    Vector3 finalPosition;

    finalPosition = targetPosition;

    // inside your Update function
    if (smoothMotion) {

 
    xDifference = Mathf.Abs(targetPosition.x - currentPosition.x);
    yDifference = Mathf.Abs(targetPosition.y - currentPosition.y);

    if (xDifference >= xThreshold || yDifference >= yThreshold) {
      finalPosition = Vector3.Lerp(currentPosition, targetPosition, smoothMotionSpeed * Time.deltaTime);
    }
    finalPosition.x = (xDifference < xThreshold) ? currentPosition.x : finalPosition.x;
    finalPosition.y = (yDifference < yThreshold) ? currentPosition.y : finalPosition.y;
    //finalPosition.z = lockZ ? currentPosition.z : finalPosition.z;
    transform.position = finalPosition;
    } else {
      finalPosition = targetPosition;
    }
  }
}
