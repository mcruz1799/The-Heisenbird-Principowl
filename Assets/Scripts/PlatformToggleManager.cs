using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformToggleManager : MonoBehaviour {
  private static PlatformToggleManager _s;
  public static PlatformToggleManager S {
    get => _s == null ? throw new System.Exception("PlatformToggleManager not initialized") : _s;
    set => _s = value;
  }

  private ISet<Platform> platforms = new HashSet<Platform>();

  private void Awake() {
    S = this;
  }

  public bool AddPlatform(Platform platform) {
    return platforms.Add(platform);
  }

  public bool RemovePlatform(Platform platform) {
    return platforms.Remove(platform);
  }

  public void Toggle(PlatformToggleGroup colorGroup) {
    SoundManager.S.PlatformToggle();
    foreach (Platform platform in platforms) {
      if (colorGroup != PlatformToggleGroup.None && platform.ColorGroup == colorGroup) {
        platform.IsActive = !platform.IsActive;
      }
    }
  }
}
