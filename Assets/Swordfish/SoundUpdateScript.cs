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
        //we need to find the audio toggle and check it
        //Graph Creator/MainMenuAnchor/GraphMenuCanvas/AudioToggle
        GameObject main = transform.parent.gameObject.transform.parent.gameObject;

        //main.transform.Find
        GameObject toggle = GameObject.FindGameObjectWithTag("Audio");
        soundControl = toggle.GetComponent<Toggle>();

        //get proportion of graph completed.
        
        createSound.Play();


    }

    // Update is called once per frame
    void Update()
    {
        if (soundControl.isOn)
        {
            createSound.pitch = gConfig.getGraphUpdateProgress() +1;
            createSound.Play();
        }
        else
        {
            //remove after testing
            //createSound.pitch = gConfig.getGraphUpdateProgress() + 1;
            ///createSound.Play();
        }
    }

    [ContextMenu("SoundTest")]
    public void increasePitch()
    {
        createSound.pitch = gConfig.getGraphUpdateProgress() + 1;
        createSound.Play();
    }
}
