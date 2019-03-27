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

  public static void Toggle(PlatformToggleGroup colorGroup) {
    foreach (Platform platform in platforms) {
      if (colorGroup != PlatformToggleGroup.None && platform.ColorGroup == colorGroup) {
        if (platform.IsActive) SoundManager.S.PlatformToggleOff();
        else SoundManager.S.PlatformToggleOn();
        platform.IsActive = !platform.IsActive;
      }
    }
  }
}
