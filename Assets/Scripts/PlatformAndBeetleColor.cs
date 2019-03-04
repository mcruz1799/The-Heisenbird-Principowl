using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformAndBeetleColor { None, Cyan }

public static class PlatformAndBeetleColorExtensions {
  public static Color32 RgbColor(this PlatformAndBeetleColor color) {
    switch (color) {
      case PlatformAndBeetleColor.Cyan:
        return Color.cyan;

      default:
        throw new System.ArgumentException("Illegal enum value");
    }
  }
}
