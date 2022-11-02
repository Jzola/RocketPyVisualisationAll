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

    public void textUpdate(float value)
    {
        if (sliderText)
            sliderText.text = value.ToString();
    }
}