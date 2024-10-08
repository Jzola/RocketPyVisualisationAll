using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableSelection : MonoBehaviour
{
    private SelectVariable[] toggles;
    void OnEnable()
    {
        toggles = GetComponentsInChildren<SelectVariable>();
    }

    public List<string> GetSelection()
    {
        List<string> selection = new List<string>();
        foreach (SelectVariable toggle in toggles)
        {
            if (toggle.selected)
                selection.Add(toggle.VariableName);
        }

        return selection;
    }

    public void ClearSelection()
    {
        foreach (SelectVariable toggle in toggles)
        {
            if (toggle.selected)
                toggle.toggleSelection();
        }
    }
}
