using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratsScreenBehaviour : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    public CanvasGroup uiCanvasGroup;
    public CanvasGroup confirmQuitCanvasGroup;
    public AudioClip sound;
    private AudioSource source {get{return GetComponent<AudioSource>();}}

    // ============================== START ==============================
    void Start(){
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;
    }

    private void Awake() {
        // Disable the quit confirmation panel
        DoConfirmQuitNo();
    }

    // ============================== BUTTON CALLBACKS ==============================
    
    public void DoConfirmQuitNo() {

        // Enable the normal UI
        uiCanvasGroup.alpha = 1;
        uiCanvasGroup.interactable = true;
        uiCanvasGroup.blocksRaycasts = true;

        // Disable the confirmation quit UI
        confirmQuitCanvasGroup.alpha = 0;
        confirmQuitCanvasGroup.interactable = false;
        confirmQuitCanvasGroup.blocksRaycasts = false;

    }

    // Called if clicked on Yes (confirmation)
    public void OnConfirmQuit() {
        Debug.Log("Game closed!");
        Application.Quit();
    }

    public void OnCancelClick() {
        DoConfirmQuitNo();
    }

    // Called if clicked on Quit
    public void OnQuitButtonClick() {
        // Reduce the visibility of normal UI, and disable all interraction
        uiCanvasGroup.interactable = false;
        uiCanvasGroup.blocksRaycasts = false;

        // Enable interraction with confirmation UI and make visible
        confirmQuitCanvasGroup.alpha = 1;
        confirmQuitCanvasGroup.interactable = true;
        confirmQuitCanvasGroup.blocksRaycasts = true;

    }
    
    // ============================== PLAY BUTTON SOUND ==============================
    public void playSound(){
        source.PlayOneShot(sound);
    }

}