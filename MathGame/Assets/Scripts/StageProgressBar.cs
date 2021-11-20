using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageProgressBar : MonoBehaviour {
    
    // ============================== INTERNAL VARIABLES ==============================

    // The actual progress bar sections UI components
    public Image[] sections;

    public Color completedColor;
    public Color inProgressColor;
    public Color notCompletedColor;

    // The last stage that was updated to be the value of the progress bar
    int previousStage = -1;

    // ============================== START ==============================

    private void Start() {
        // Get the progress bar sections UI components
        sections = GetComponentInChildren<HorizontalLayoutGroup>().gameObject.GetComponentsInChildren<Image>();
    }

    // ============================== UPDATE ==============================

    private void Update() {

        // Update the progress bar to current stage number when it changes
        if(previousStage != MainController.stageNumber) {
            previousStage = MainController.stageNumber;
            
            for(int i = 0; i < sections.Length; i++) {
                if(i < MainController.stageNumber) {
                    sections[i].color = completedColor;
                } else if(i == MainController.stageNumber) {
                    sections[i].color = inProgressColor;
                } else {
                    sections[i].color = notCompletedColor;
                }
            }

        }

    }

}
