using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PressAnyKeyToStart : MonoBehaviour {
    bool didPlaySound = false;
    public AudioClip sound;
    private AudioSource source {get{return GetComponent<AudioSource>();}}
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;
    }

    // ============================== UPDATE ==============================

    void Update() {

        // If any input is made, switch to the menu scene
        if (Input.anyKey && !didPlaySound) {
            source.PlayOneShot(sound);
            didPlaySound = true;
            Invoke("Proceed",sound.length);
        }

    }

    public void Proceed(){
        SceneManager.LoadScene("Menu Screen");
    }
}