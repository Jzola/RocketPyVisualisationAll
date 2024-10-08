using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableSelectionToggle : Button
{
    [field: SerializeField]
    public string VariableName;
    [SerializeField]
    private bool test;
}
