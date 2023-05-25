using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundUpdateScript : MonoBehaviour
{
    public AudioSource createSound;
    public Toggle soundControl;
    public GraphConfig gConfig;
    // Start is called before the first frame update
    void Start()
    {
        // Get the sound controller from root
        GameObject toggle = GameObject.FindGameObjectWithTag("Audio");
        soundControl = toggle.GetComponent<Toggle>();

        createSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (soundControl.isOn)
        {
            // Repeats sounds while graph is updating/ generating.
            if (gConfig.getGraphUpdateProgress() < 1.0f ){
                createSound.pitch = gConfig.getGraphUpdateProgress() +1;
                createSound.Play();
            }
            
        }
    }

    [ContextMenu("SoundTest")]
    public void increasePitch()
    {
        createSound.pitch = gConfig.getGraphUpdateProgress() + 1;
        createSound.Play();
    }
}
