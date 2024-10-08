using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour
{
    [SerializeField]
    private Text description;

    public void SetDescription(string text)
    {
        description.text = text;
    }
}
