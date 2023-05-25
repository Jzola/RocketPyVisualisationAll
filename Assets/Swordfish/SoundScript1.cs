using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundScript1 : MonoBehaviour
{
    
    public AudioSource createGraphSound;
    //public AudioClip createGraphSound;
    public Button createGraphButton;
    public Toggle soundEnabled;
    private float startingPitch = 4;
    public int timeToDecrease = 5; 
    //private bool soundEnabled;

    private void Start()
    {
        createGraphButton.onClick.AddListener(playSound);

    }
    // Update is called once per frame
    void Update()
    {
        //if (createGraphSound.pitch > 0)
        //{
        //    createGraphSound.pitch -= Time.deltaTime * startingPitch / timeToDecrease;
        //}
    }
    [ContextMenu("Test sound")]
    public void playSound()
    {
        if (soundEnabled.isOn)
            createGraphSound.Play();

        //if (createGraphSound.pitch > 0)
        //{
        //    createGraphSound.pitch -= Time.deltaTime * startingPitch / timeToDecrease;
        //}
    }
}
