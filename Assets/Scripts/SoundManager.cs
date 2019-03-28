using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  public static SoundManager S { get; private set; }

#pragma warning disable 0649
  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip headBonk;
  [SerializeField] private AudioClip landed;
  [SerializeField] private AudioClip jump;
  [SerializeField] private AudioClip damaged;
  [SerializeField] private AudioClip playerDeath;
  [SerializeField] private AudioClip beetleDeath;
  [SerializeField] private AudioClip platformToggle;
#pragma warning restore 0649


  private void Awake() {
    S = this;
    audioSource = GetComponent<AudioSource>();
  }

  public void PlayerHeadBonk() {
    audioSource.clip = headBonk;
    audioSource.Play();
  }

  public void PlayerLanded() {
    // audio.clip = Landed;
    // audio.Play();
  }

  public void PlayerJump() {
    audioSource.clip = jump;
    audioSource.Play();
  }

  public void PlayerJumpOffEnemy() {
    //NOT YET IMPLEMENTED
    PlayerJump();
  }

  public void PlayerDamaged() {
    audioSource.clip = damaged;
    audioSource.Play();
  }

  public void PlayerDied() {
    audioSource.clip = playerDeath;
    audioSource.Play();
  }
  public void BeetleDied(){
    audioSource.clip = beetleDeath;
    audioSource.Play();
  }

  public void PlatformToggle(){
    audioSource.clip = platformToggle;
    audioSource.Play();
  }
}
