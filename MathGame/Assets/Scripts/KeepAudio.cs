using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake(){
        if(GameObject.FindGameObjectsWithTag("BG Audio").Length > 1) {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);
    }

    void Update(){
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "Congratulations Screen" || sceneName == "Game Screen" )
            Destroy(this.gameObject);
    }
}
