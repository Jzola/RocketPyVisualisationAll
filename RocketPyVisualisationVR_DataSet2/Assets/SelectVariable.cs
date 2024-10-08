using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectVariable : MonoBehaviour
{
    public string VariableName;

    [SerializeField]
    private Graphic graphic;
    [SerializeField]
    private float dimFactor = 0.9f;

    public bool selected;

    public void toggleSelection()
    {
        selected = !selected;
    }
}
