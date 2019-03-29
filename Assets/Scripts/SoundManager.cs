using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  public static SoundManager S { get; private set; }

#pragma warning disable 0649
  [SerializeField] private AudioClip headBonk;
  [SerializeField] private AudioClip landed;
  [SerializeField] private AudioClip jump;
  [SerializeField] private AudioClip damaged;
  [SerializeField] private AudioClip playerDeath;
  [SerializeField] private AudioClip beetleDeath;
  [SerializeField] private AudioClip platformToggle;
#pragma warning restore 0649

  private Stack<AudioSource> audioSources = new Stack<AudioSource>();

  private void Awake() {
    S = this;
    for (int i = 0; i < 5; i++) {
      GameObject g = new GameObject("AudioSource");
      g.transform.parent = transform;
      audioSources.Push(g.AddComponent<AudioSource>());
    }
  }

  private void PlaySound(AudioClip sound) {
    AudioSource src = audioSources.Pop();
    src.clip = sound;
    src.Play();
    audioSources.Push(src);
  }

  public void PlayerHeadBonk() {
    PlaySound(headBonk);
  }

  public void PlayerLanded() {
    // audio.clip = Landed;
    // audio.Play();
  }

  public void PlayerJump() {
    PlaySound(jump);
  }

  public void PlayerJumpOffEnemy() {
    //NOT YET IMPLEMENTED
    PlayerJump();
  }

  public void PlayerDamaged() {
    PlaySound(damaged);
  }

  public void PlayerDied() {
    PlaySound(playerDeath);
  }

  public void BeetleDied(){
    PlaySound(beetleDeath);
  }

  public void PlatformToggle(){
    PlaySound(platformToggle);
  }
}
