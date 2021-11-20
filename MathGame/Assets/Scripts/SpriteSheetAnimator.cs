using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetAnimator : MonoBehaviour {

    // ============================== INSPECTOR VARIABLES ==============================

    // The renderer component containing the material whose offset to animate
    public Renderer rendererComponent;

    // If need to start playing right away
    public bool autoStart = true;

    // The number of frames in current spritesheet and playback speed
    public int numberOfFrames = 3;
    public float framesPerSecond = 10.0f;

    // The frame to display when animaiton is stopped
    public int stoppedFrame = 1;

    // The textures to switch out when needed (different animations)
    public Texture[] textures;

    // ============================== INTERNAL VARIABLES ==============================

    // Keep track of the previous frame to only switch texture when frame switches
    int previousFrame = -1;

    // Keep track of the previous texture to only switch when need be
    int previousTextureIndex = -1;

    // Is the animation currently playing?
    bool isPlaying;

    // Size of every frame
    Vector2 size;

    // ============================== START ==============================

    void Start() {

        // Pre-calculate size of every frame
        size = new Vector2(1.0f / numberOfFrames, 1.0f);

        // Start playing animation if auto-start is true
        isPlaying = autoStart;

    }

    // ============================== UPDATE ==============================

    void Update() {

        // If the animation is currently playing
        if (isPlaying) {

            // Calculate current frame
            int frame = (int) (Time.time * framesPerSecond) % numberOfFrames;

            // If frame switched
            if (frame != previousFrame) {
                previousFrame = frame;
                // Display the current frame
                setFrame(frame);
            }

        }

    }

    void setFrame(int frame) {
        // Set the texture offset to display the current frame
        Vector2 offset = new Vector2(frame * size.x, 0);
        rendererComponent.material.mainTextureOffset = offset;
    }

    // ============================== START / STOP ==============================

    public void start() {
        // Start playing
        isPlaying = true;
    }

    public void stop() {
        // Stop playing and display the stopped frame
        isPlaying = false;
        setFrame(stoppedFrame);
    }

    // ============================== SET TEXTURE ==============================

    public void setTexture(int textureIndex) {

        // If texture switched
        if (textureIndex != previousTextureIndex) {
            previousTextureIndex = textureIndex;

            // Set the material's texture to the required one
            rendererComponent.material.mainTexture = textures[textureIndex];

        }

    }

}