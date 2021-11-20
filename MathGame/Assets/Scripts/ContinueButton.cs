using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour {
    public AudioClip sound;
    private AudioSource source {get{return GetComponent<AudioSource>();}}
    // ============================== BUTTON CALLBACK ==============================

    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;
    }

    public void onContinueButtonClick() {
        source.PlayOneShot(sound);
        // Load the menu scene when the continue button is clicked
        Invoke("Proceed", sound.length);
    }

    void Proceed(){
        SceneManager.LoadScene("Menu Screen");
    }
}