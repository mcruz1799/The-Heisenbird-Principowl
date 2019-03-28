using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  public static SoundManager S { get; private set; }

#pragma warning disable 0649
  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip HeadBonk;
  [SerializeField] private AudioClip Landed;
  [SerializeField] private AudioClip Jump;
  [SerializeField] private AudioClip Damaged;
  [SerializeField] private AudioClip PlayerDeath;
  [SerializeField] private AudioClip BeetleDeath;
  [SerializeField] private AudioClip PlatformOn;
  [SerializeField] private AudioClip PlatformOff;
#pragma warning restore 0649


  private void Awake() {
    S = this;
    audioSource = GetComponent<AudioSource>();
  }

  public void PlayerHeadBonk() {
    audioSource.clip = HeadBonk;
    audioSource.Play();
  }

  public void PlayerLanded() {
    // audio.clip = Landed;
    // audio.Play();
  }

  public void PlayerJump() {
    audioSource.clip = Jump;
    audioSource.Play();
  }

  public void PlayerJumpOffEnemy() {
    //NOT YET IMPLEMENTED
    PlayerJump();
  }

  public void PlayerDamaged() {
    audioSource.clip = Damaged;
    audioSource.Play();
  }

  public void PlayerDied() {
    audioSource.clip = PlayerDeath;
    audioSource.Play();
  }
  public void BeetleDied(){
    audioSource.clip = BeetleDeath;
    audioSource.Play();
  }

  public void PlatformToggleOn(){
    audioSource.clip = PlatformOn;
    audioSource.Play();
  }

  public void PlatformToggleOff(){
    audioSource.clip = PlatformOff;
    audioSource.Play();
  }
}
