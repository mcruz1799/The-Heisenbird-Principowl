using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
  private bool isPanning;
  private PanTargets currentTargets;
  private Vector3 currentVelocity;

  private void Awake() {
    if (!useInspectorOffset) {
      localOffset = transform.position - toFollow.position;
    }
  }

  private void LateUpdate() {
    if (!isPanning) {
      //Once here, assumes that it is following the Player.
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

  //Smoothly pans the camera between a set of transform positions.
  //Transforms && TimesBetween sizes must be equivalent.
  //Final transform & TimeBetween should be player.
  private List<Tuple<Transform, float>> CameraPanTargets(Transform[] transforms, float[] timesBetween)
  {
    List<Tuple<Transform, float>> panTargets = new List<Tuple<Transform, float>>();
    isPanning = true;
    for (int i = 0; i < transforms.Length; i++) {
      panTargets.Add(new Tuple<Transform, float>(transforms[i], timesBetween[i]));
    }
    return panTargets;
  }

  public void PanCamera(Transform[] transforms, float[] timesBetween)
  {
    currentTargets.Reset();
    currentTargets._targets = CameraPanTargets(transforms, timesBetween);
  }


  public void PanNext()
  {
    if (currentTargets.MoveNext()) {
      Tuple<Transform, float> target = currentTargets.Current;
       Vector3 currentPosition = transform.position;
      Vector3 targetPosition = target.Item1.position + localOffset;

      transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref currentVelocity, target.Item2);
    } else Debug.Log("No Targets left to pan to.");

  }

  public void StopPanning()
  {
    isPanning = false;
    currentTargets.Reset();
    currentTargets = null;
  }

  public void PanToPlayer(float TimeBetween)
  {
    Vector3 currentPosition = transform.position;
    Vector3 targetPosition = toFollow.position + localOffset;

    transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref currentVelocity, TimeBetween);
    isPanning = false;
  }


  //** Enumeration class for the camera pan targets. **//
  private class PanTargets: IEnumerator
  {
    public List<Tuple<Transform, float>> _targets;
    int position = -1;

    public bool MoveNext()
    {
      position++;
      return (position < _targets.Count);
    }

    public void Reset()
    {
      position = -1;
    }

    object IEnumerator.Current {
      get {
        return Current;
      }
    }

    public Tuple<Transform,float> Current {
      get {
        try {
          return _targets[position];
        } catch (IndexOutOfRangeException) {
          throw new InvalidOperationException();
        }
      }
    }
  }

}



