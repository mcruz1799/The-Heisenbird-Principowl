using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Direction;

public enum Direction { North, South, East, West }
public static class DirectionExtensions {
  public static Direction Opposite(this Direction direction) {
    switch (direction) {
      case North:
        return South;

      case South:
        return North;

      case East:
        return West;

      case West:
        return East;

      default:
        throw new System.ArgumentException("Illegal direction enum value");
    }
  }
}