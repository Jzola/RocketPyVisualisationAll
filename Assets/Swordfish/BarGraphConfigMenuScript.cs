using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarGraphConfigMenuScript : MonoBehaviour
{
    private BarGraphConfig gConfig;
    public OutputVariableVisibility outputConfig;
    public GameObject dataScrollViewContent;
    public TextAsset variableExtractionFile;
    public GameObject toggleTemplate;
    public Dropdown inputVariableDropdown;
    public int testValue = 2;
    public Button reconfigButton;

    private List<string> variables;
    public List<GameObject> dataToggles;


    // Start is called before the first frame update
    void Start()
    {
        //delete button is already handled separately, we only need the config GameObject.
        gConfig = transform.parent.gameObject.GetComponent<BarGraphConfig>();

        // Creates a list of variables from the given file
        variables = new List<string>();
        variables.AddRange(variableExtractionFile.text.Substring(0, variableExtractionFile.text.IndexOf(System.Environment.NewLine)).Split(','));


        setupDataDisplay();

        //set up the variables shown in the input drop down
        setUpDropdown(inputVariableDropdown, gConfig.availableInputFolders, 2);

        //set up the graph update button with a listener
        reconfigButton.onClick.AddListener(updateGraph);

    }

    // Update is called once per frame
    void Update()
    {

    }
    [ContextMenu("Update graph")]
   public void updateGraph()
    {
        gConfig.replaceBarGraph();
    }

    private void setupDataDisplay()
    {
        //get the number of possible output variables from graph config. Default as of May 2023 is 49 variables
        int noToggles = variables.Count;

        int index;
        //add the toggles to the data panel and connect them to a listener that ajdusts variable visibility
        foreach (string vars in variables)
        {

            index = variables.IndexOf(vars);
            bool[] visibilities = outputConfig.getVisibilities();

            GameObject toggle = (GameObject)Instantiate(toggleTemplate);
            //ensure the toggle instantiates according to parent, not world value by setting second argument to false.
            toggle.transform.SetParent(dataScrollViewContent.transform, false);
            //set toggle label
            toggle.GetComponentInChildren<Text>().text = vars;

            //add listener
            toggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate { outputVisibilityListener(toggle.GetComponent<Toggle>(), index); });

            dataToggles.Add(toggle);

            //use the index to determine if the toggle should be on or off at start.
            if (!visibilities[index])
            {
                toggle.GetComponent<Toggle>().isOn = false;
            }


        }


    }

    //will activate whenever the toggle value changes
    public void outputVisibilityListener(Toggle tog, int index)
    {
        outputConfig.setVisibility(tog.GetComponentInChildren<Text>().text, tog.isOn);
    }
    private void setUpDropdown(Dropdown dDown, List<string> options, int defaultIndex)
    {
        dDown.options.Clear();
        dDown.AddOptions(options);
        dDown.value = gConfig.availableInputFolders.IndexOf(gConfig.inputFolderName);
        dDown.onValueChanged.AddListener(delegate { dropdownItemSelected(dDown); });

    }
    [ContextMenu("Test Dropdown")]
    public void testDropdown()
    {
        if(testValue < 10)
        {
            inputVariableDropdown.value = testValue;
        }
    }

    private void dropdownItemSelected(Dropdown axisDropdown)
    {
        int index = axisDropdown.value;

        if (axisDropdown.Equals(inputVariableDropdown))
        {
            gConfig.inputFolderName = axisDropdown.options[index].text;
        }
    }
}
