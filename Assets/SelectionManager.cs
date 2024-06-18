using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//For keeping track of the selected rocket
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

    //Change the selected rocket and update text
    public void ChangeSelection(string rocketId)
    {
        if (rocketId != selectedRocketId)
        {
            selectedRocketId = rocketId;
            rocketNameDisplay.text = rocketId;
            confirmButton.interactable = true;
        }
        //Pressing the same selection button twice deselects the rocket
        else
        {
            rocketNameDisplay.text = "None";
            selectedRocketId = "";
            confirmButton.interactable = false;
        }
    }

    //Upon confirmation show the completion panel with the selected rocket
    //TODO: Once metrics collecting is added should stop collecting here
    public void ConfirmSelection()
    {
        completionPanel.SetActive(true);
        finalSelectionText.text = selectedRocketId;
    }


}
