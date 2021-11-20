using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour {

    
    public AudioClip sound;
    private AudioSource source {get{return GetComponent<AudioSource>();}}
    // ============================== START ==============================

    private void Start() {
        // Get the button UI component and automatically add the callback
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;
        GetComponent<Button>().onClick.AddListener(delegate { onStartButtonClick(); });
    }

    // ============================== BUTTON CALLBACK ==============================

    public void onStartButtonClick() {

        // Switch to the game scene when the start button is pressed
        // TODO: Make this work for the other game modes and levels
        if ((MainController.selectedLevel == Level.LEVEL1) && (MainController.onePlayerMode == true)) {
           source.PlayOneShot(sound);
           Invoke("Proceed",sound.length);
        }

    }

    public void Proceed(){
        SceneManager.LoadScene("Game Screen");
    }
}