using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LivesIndicator : MonoBehaviour {
    // ============================== INTERNAL VARIABLES ==============================

    // The actual progress bar sections UI components
    public Image[] hearts;

    // The last stage that was updated to be the value of the progress bar
    int prevNumLives = -1;

    // ============================== START ==============================

    private void Start() {
        // Get the progress bar sections UI components
        hearts = GetComponentInChildren<HorizontalLayoutGroup>().gameObject.GetComponentsInChildren<Image>();
    }

    // ============================== UPDATE ==============================

    private void Update() {

        // Update the progress bar to current stage number when it changes
        if(prevNumLives != MainController.leftPlayer.numLives) {
            prevNumLives = MainController.leftPlayer.numLives;
            
            for(int i = 0; i < hearts.Length; i++) {
                if(i >= MainController.leftPlayer.numLives){
                    hearts[i].enabled = false;
                } else {
                    hearts[i].enabled = true;
                }
            }
            
        }

    }
}
