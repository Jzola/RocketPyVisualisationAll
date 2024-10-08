using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleAltitudeCheck : MonoBehaviour
{
    private AltitudeCheck[] altitudeChecks;
    [SerializeField]
    private Toggle altitudeToggle;

    public GameObject VisualisationParent;

    // Start is called before the first frame update
    void Start()
    {
        altitudeChecks = VisualisationParent.GetComponentsInChildren<AltitudeCheck>(true);

        altitudeToggle.onValueChanged.AddListener(delegate
        {
            toggleAltitude(altitudeToggle);
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
