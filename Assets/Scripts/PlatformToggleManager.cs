using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformToggleManager {
  private static ISet<Platform> platforms = new HashSet<Platform>();

  public static bool AddPlatform(Platform platform) {
    return platforms.Add(platform);
  }

  public static bool RemovePlatform(Platform platform) {
    return platforms.Remove(platform);
  }

  public static void Toggle(PlatformAndBeetleColor colorGroup) {
    foreach (Platform platform in platforms) {
      if (colorGroup != PlatformAndBeetleColor.None && platform.ColorGroup == colorGroup) {
        platform.IsActive = !platform.IsActive;
      }
    }
  }
}
