using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    // The level that the button will set as active
    public Level levelToSet;

    // ============================== INTERNAL VARIABLES ==============================

    // The actual toggle button UI component
    Toggle toggle;

    // ============================== START ==============================

    private void Start() {
        // Get the toggle button UI component and automatically add the callback
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate { onToggled(toggle); });
    }

    // ============================== BUTTON CALLBACK ==============================

    public void onToggled(Toggle toggle) {
        // If the button is now selected, set the corresponding level
        if (toggle.isOn) MainController.selectedLevel = levelToSet;
    }

}