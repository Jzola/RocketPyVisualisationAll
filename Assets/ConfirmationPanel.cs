using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPanel : MonoBehaviour
{
    [SerializeField]
    private Text rocketIDText;

    public void SetRocketID(int id)
    {
        rocketIDText.text = "Rocket" + id + "?";
    }
}
