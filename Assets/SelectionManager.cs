using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    private string selectedRocketId;

    [SerializeField]
    private Text rocketNameDisplay;
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private GameObject completionPanel;
    [SerializeField]
    private Text finalSelectionText;

    public void ChangeSelection(string rocketId)
    {
        if (rocketId != selectedRocketId)
        {
            selectedRocketId = rocketId;
            rocketNameDisplay.text = rocketId;
            confirmButton.interactable = true;
        }
        else
        {
            rocketNameDisplay.text = "None";
            selectedRocketId = "";
            confirmButton.interactable = false;
        }
    }

    public void ConfirmSelection()
    {
        completionPanel.SetActive(true);
        finalSelectionText.text = selectedRocketId;
    }


}
