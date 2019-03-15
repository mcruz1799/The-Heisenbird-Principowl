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
    //Check if there is already an S of SoundManager
    if (S == null)
        //if not, set it to this.
        S = this;
    //If S already exists:
    else if (S != this)
        //Destroy this, this enforces our singleton pattern so there can only be one S of SoundManager.
        Destroy (gameObject);
    //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
    DontDestroyOnLoad (gameObject);
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
