using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SidePanel : MonoBehaviour
{
    [SerializeField]
    private Text rocketIDText;
    [SerializeField]
    private Text scenarioIDText;
    [SerializeField]
    private Button selectButton;
    [SerializeField]
    private ConfirmationPanel confirmationPanel;

    private int selectedRocket;

    // Start is called before the first frame update
    void Start()
    {
        selectButton.onClick.AddListener(confirmSelection);
    }

    private void confirmSelection()
    {
        confirmationPanel.gameObject.SetActive(true);
        confirmationPanel.SetRocketID(selectedRocket);
    }

    public void SetRocketID(int id)
    {
        selectedRocket = id;

        if (selectedRocket == 1)
        {
            rocketIDText.text = "Base Specifications";
        }
        if (selectedRocket == 2)
        {
            rocketIDText.text = "Change Mass";
        }
        if (selectedRocket == 3)
        {
            rocketIDText.text = "Change Nozzle";
        }
        if (selectedRocket == 4)
        {
            rocketIDText.text = "Change Propellant";
        }
        if (selectedRocket == 5)
        {
            rocketIDText.text = "Change Nose to CM";
        }
        if (selectedRocket == 6)
        {
            rocketIDText.text = "Change Burnout Time";
        }
    }

    public void SetScenarioID(string text)
    {
        scenarioIDText.text = text;
    }
}
