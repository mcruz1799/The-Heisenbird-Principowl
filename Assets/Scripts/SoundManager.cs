using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  public static SoundManager S { get; private set; }
  private AudioSource audio;
  public AudioClip HeadBonk;
  public AudioClip Landed;
  public AudioClip Jump;
  public AudioClip Damaged;
  public AudioClip Died;

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
    audio = GetComponent<AudioSource>();
  }

  public void PlayerHeadBonk() {
    audio.clip = HeadBonk;
    audio.Play();
  }

  public void PlayerLanded() {
    // audio.clip = Landed;
    // audio.Play();
  }

  public void PlayerJump() {
    audio.clip = Jump;
    audio.Play();
  }

  public void PlayerDamaged() {
    audio.clip = Damaged;
    audio.Play();
  }

  public void PlayerDied() {
    audio.clip = Died;
    audio.Play();
  }
}
