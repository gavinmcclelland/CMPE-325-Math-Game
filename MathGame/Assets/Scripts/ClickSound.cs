using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickSound : MonoBehaviour
{
    public AudioClip sound;
    private AudioSource source {get{return GetComponent<AudioSource>();}}
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey) {
            source.PlayOneShot(sound);
            SceneManager.LoadScene("Menu Screen");
        }
    }
}
