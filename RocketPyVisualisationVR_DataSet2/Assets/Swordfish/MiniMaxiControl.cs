using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zinnia.Action;

public class MiniMaxiControl : MonoBehaviour
{
    public BooleanAction trigger;
    public MaximizeButton maximiseButton;
    private bool previousState;

    // Start is called before the first frame update
    void Start()
    {
        previousState = trigger.IsActivated;
    }

    // Update is called once per frame
    void Update()
    {
        if (justTriggered())
        {
            maximiseButton.ToggleCanvasVisibility();
        }
        previousState = trigger.IsActivated;
    }

    // Checks if the state has been triggered from the the last check, returns true if it has
    private bool justTriggered()
    {
        return !previousState && trigger.IsActivated;
    }
}
