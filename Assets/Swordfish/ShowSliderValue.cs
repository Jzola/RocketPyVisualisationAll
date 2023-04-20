using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSliderValue : MonoBehaviour
{
    Text sliderText;
    void Start()
    {
        sliderText = GetComponent<Text>();
    }

    //update text after user change
    public void textUpdate(float value)
    {
        if (sliderText)
            sliderText.text = value.ToString();
    }
}