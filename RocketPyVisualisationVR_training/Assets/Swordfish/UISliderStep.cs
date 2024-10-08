using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach this script to a Unity.UI Slider component to control the step size within the slider range.
/// </summary>
[RequireComponent(typeof(Slider))]
public class UISliderStep : MonoBehaviour
{
    private Slider slider;
    private List<float> stepValues;

    private bool init = false;

    private void Update()
    {
        if (!init && stepValues != null)
        {
            slider = GetComponent<Slider>();
            if (slider != null)
            {
                slider.onValueChanged.AddListener(ClampSliderValue);
                slider.maxValue = stepValues.Max();
                slider.minValue = stepValues.Min();
                init = true;
            }
        }
    }

    public void ClampSliderValue(float value)
    {
        if (slider != null && stepValues.Count > 0)
        {
            if (!stepValues.Contains(value))
            {
                float closest = stepValues.Aggregate((x, y) => Math.Abs(x - value) < Math.Abs(y - value) ? x : y);
                slider.value = closest;
            }
        }
    }

    public void SetSliderValues(List<float> values)
    {
        stepValues = values;        
    }
}

