using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AltitudeCheckToggle : MonoBehaviour
{
    public GameObject visualisationParent;

    private Toggle toggle;
    private List<AltitudeCheck> altitudeChecks;


    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        altitudeChecks = new List<AltitudeCheck>(visualisationParent.GetComponentsInChildren<AltitudeCheck>(true));

        toggle.onValueChanged.AddListener(delegate
        {
            toggleAltitude(toggle);
        });
    }

    void toggleAltitude(Toggle change)
    {
        foreach (AltitudeCheck altitudeCheck in altitudeChecks)
        {
            altitudeCheck.gameObject.SetActive(change.isOn);
        }
    }
    
}
