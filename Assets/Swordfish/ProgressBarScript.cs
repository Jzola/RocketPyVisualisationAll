using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarScript : MonoBehaviour
{
    public Slider bar;
    public GraphConfig gConfig;

    // Start is called before the first frame update
    void Start()
    {
        if (bar == null)
        {
            bar = transform.GetComponentInChildren<Slider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        bar.value = gConfig.getGraphUpdateProgress();
        checkFinished();
    }

    public void checkFinished()
    {
      //if finished, deactivate the object (will be ivisible and not interactable)
        bar.gameObject.SetActive(gConfig.getGraphUpdateProgress() != 1.0f);
    }
}
