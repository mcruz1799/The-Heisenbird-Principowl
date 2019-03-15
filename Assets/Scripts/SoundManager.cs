using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  public static SoundManager S { get; private set; }
  private AudioSource audioSource;
  public AudioClip HeadBonk;
  public AudioClip Landed;
  public AudioClip Jump;
  public AudioClip Damaged;
  public AudioClip PlayerDeath;
  public AudioClip BeetleDeath;
  

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
}
