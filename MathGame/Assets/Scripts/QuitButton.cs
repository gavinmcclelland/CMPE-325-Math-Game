using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour {

    public AudioClip sound;
    private AudioSource source {get{return GetComponent<AudioSource>();}}
    
    // ============================== START ==============================
    private void Start() {
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;
        // Get the button UI component and automatically add the callback
        GetComponent<Button>().onClick.AddListener(delegate { onQuitButtonClick(); });
    }

    // ============================== BUTTON CALLBACK ==============================

    public void onQuitButtonClick() {
        // Close the game when the quit button is pressed
        Debug.Log("Game closed!");
        source.PlayOneShot(sound);
        Application.Quit();
    }
}