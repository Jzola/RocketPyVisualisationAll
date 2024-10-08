using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using IATK;
using System;
using System.Linq;

public class GraphCreatorMenuScript : MonoBehaviour
{
    //references to create graph and graph config will be required
    public GraphCreator gCreator;

    public Dropdown xaxisDropdown;
    public Dropdown yaxisDropdown;
    public Dropdown zaxisDropdown;
    public Dropdown variableDropdown;
    public ToggleGroup graphTypeChoice;
    public ToggleGroup dimensionsChoice;
    public Text debugText;
    public Button createGraphButton;
    public List<string> axesChosen = new List<string>();
    public string xaxisChosen;
    public string yaxisChosen;
    public string zaxisChosen;
    public string inputvariableChosen;
    public GraphCreator.GraphType graphTypeChosen;
    public string dimensionChosen;
    public List<Toggle> graphToggles;
    public List<Toggle> dimensionToggles;
    private MeshRenderer rend;
    private int dropDownValue;
    public string dropdownTester="";
    public List<string> variables;
    private int defaultIndex = 1;
    private int interval = 3;
    private string defaultFolder = "Default_Inputs";

    public enum axisdropDowns {xaxis, yaxis, zaxis };
    public axisdropDowns axisDropdowns = axisdropDowns.xaxis;


    private bool graphsCreatable= true;


    // Start is called before the first frame update
    void Start()
    {
        //this folder should always be present
        String defaultFolder = "Default_Inputs";


        //check where default inputs is, as the folder structure can be changed by adding more datasets.
        if (gCreator.availableInputs.Contains(defaultFolder))
        {
            defaultIndex = gCreator.availableInputs.IndexOf(defaultFolder);
        }

        Canvas canvas = this.GetComponent<Canvas>();
        //get the anchor and make it invisible to start
        GameObject anchor = this.transform.parent.gameObject;
        rend = anchor.GetComponent<MeshRenderer>();
        rend.enabled = false;
        //set up variables for the dropdowns using the gCreator
        List<string> variableList = gCreator.variables;

        //reset dropdowns to remove 'option a' text
        xaxisDropdown.options.Clear();
        yaxisDropdown.options.Clear();
        zaxisDropdown.options.Clear();

        //fill the dropdowns
        xaxisDropdown.AddOptions(variableList);
        yaxisDropdown.AddOptions(variableList);
        zaxisDropdown.AddOptions(variableList);

        //assign listeners.
        xaxisDropdown.onValueChanged.AddListener(delegate { axisDropdownItemSelected(xaxisDropdown); });
        yaxisDropdown.onValueChanged.AddListener(delegate { axisDropdownItemSelected(yaxisDropdown); });
        zaxisDropdown.onValueChanged.AddListener(delegate { axisDropdownItemSelected(zaxisDropdown); });


        //populate the variable drop down menu
        variableDropdown.options.Clear();
        variableDropdown.AddOptions(gCreator.availableInputs);

        variableDropdown.onValueChanged.AddListener(delegate { variableDropdownItemSelected(variableDropdown); });

        debugText.text = "";
        debugText.enabled = false; //if not using the debug text, set to false.


        //add a listener to the graph dimensions options/toggles that disables z axis if 2D is checked, and enables if 3D is checked.
        //(value changes again automatically if the other toggle is clicked, due to toggle group rules, so we only need one listener)
        dimensionToggles[0].onValueChanged.AddListener(dimensionChanged);

        //add a listener to graph type toggles that disables z axis if 'bar' is checked, and enables if unchecked
        graphToggles[0].onValueChanged.AddListener(delegate { graphTypeChanged(); });

        setDefaults();

        //connect the button to the createGraph method
        createGraphButton.onClick.AddListener(createGraph);

        variables = gCreator.variables;
        //set a default value for the dropdown testing function
        dropdownTester = variables[3];




    }
    //to be used in desktop mode
    [ContextMenu("Change type")]
    public void changeGraphType()
    {
        //allow the toggle to be switched off temporarily in order to bypass the 'only one toggle isOn' rule for groupToggle
        graphTypeChoice.allowSwitchOff = true;
        if (graphToggles[0].isOn)
        {
            graphToggles[0].isOn = false;
            //due to toggle group rules, this should automatically select the other toggle,
            graphToggles[1].isOn = true;

        }
        else
        {
            graphToggles[1].isOn = false;
            graphToggles[0].isOn = true;

        }
        graphTypeChoice.allowSwitchOff = false;
    }

    //show changing of dimensions, and trigger the listener "dimensionChanged"
    [ContextMenu("Change dimension")]
    public void manualDimensionChange()
    {
        dimensionsChoice.allowSwitchOff = true;
        if (dimensionToggles[0].isOn)
        {

            dimensionToggles[0].isOn = false;
            dimensionToggles[1].isOn = true;
        }
        else
        {
            dimensionToggles[1].isOn = false;
            dimensionToggles[0].isOn = true;
        }

        dimensionsChoice.allowSwitchOff = false;
    }
    //if the dimension of the graph is 2D, hide the dropdown for the z axis and disable it.
    public void dimensionChanged(bool arg0)
    {

        if (dimensionToggles[0].isOn)
        {
            //the 2D option has been selected, so there is no Z axis
            zaxisDropdown.enabled = false;
            //completely hides the toggle
            zaxisDropdown.gameObject.SetActive(false);
        }
        else
        {
            zaxisDropdown.enabled = true;
            //zaxisDropdown.Show();
            zaxisDropdown.gameObject.SetActive(true);
        }
    }


    //test that the dropdowns can be changed via the inspector in desktop mode
    [ContextMenu("Test Dropdown")]
    public void dropDownTest()
    {
        int value;
        //make sure the variable exists
        if (variables.IndexOf(dropdownTester) != -1)
        {
            value = variables.IndexOf(dropdownTester);
        }
        else//use the default
        {
            value = 2;
        }
        if (axisDropdowns == axisdropDowns.xaxis)
        {
            xaxisDropdown.value = value;
            axisDropdownItemSelected(xaxisDropdown);
        }
        else if(axisDropdowns == axisdropDowns.yaxis)
        {
            yaxisDropdown.value = value;
            axisDropdownItemSelected(yaxisDropdown);
        }
        else if (axisDropdowns == axisdropDowns.zaxis)
        {
            zaxisDropdown.value = value;
            axisDropdownItemSelected(zaxisDropdown);
        }
    }
    //change the input variable. Will be used in createGraph.
    private void variableDropdownItemSelected(Dropdown variableDropdown)
    {
        int index = variableDropdown.value;
        inputvariableChosen = variableDropdown.options[index].text;

    }

    [ContextMenu("Choose Axis")]
    private void axisDropdownItemSelected(Dropdown axisDropdown)
    {

        int index = axisDropdown.value;
        //do something with text
        string axis = axisDropdown.options[index].text;
        //for troubleshooting.
        if (!axesChosen.Contains(axis))
            axesChosen.Add(axisDropdown.options[index].text);
        else
            axesChosen.Remove(axis);


        if (axisDropdown.Equals(xaxisDropdown))
        {
            xaxisChosen = axisDropdown.options[index].text;
            debugText.text += "x axis" + axisDropdown.options[index].text;
        }
        else if (axisDropdown.Equals(yaxisDropdown))
        {
            yaxisChosen = axisDropdown.options[index].text;
            debugText.text += "y axis " + axisDropdown.options[index].text;
        }
        else
        {
            zaxisChosen = axisDropdown.options[index].text;
            debugText.text += "z axis " + axisDropdown.options[index].text;
        }

        //show that axes are added (optional)
        debugText.enabled = false;


    }
    //listener that disables and reenables the axis options when the type of graph is 'bar'.
    public void graphTypeChanged()
    {
        //if the type is 'bar' graph
        if (graphToggles[0].isOn)
        {
            //disable dimension toggles
            dimensionToggles[0].enabled = false;

            dimensionToggles[1].enabled = false;
            dimensionToggles[0].interactable = false;
            dimensionToggles[1].interactable = false;
            //disable the axis dropdowns
            xaxisDropdown.enabled = false;
            yaxisDropdown.enabled = false;
            zaxisDropdown.enabled = false;
            xaxisDropdown.interactable = false;
            yaxisDropdown.interactable = false;
            zaxisDropdown.interactable = false;


        }
        else
        {
            //enable dimension toggles
            dimensionToggles[0].enabled = true;
            dimensionToggles[1].enabled = true;
            dimensionToggles[0].interactable= true;
            dimensionToggles[1].interactable = true;
            //enable the axis dropdowns
            xaxisDropdown.enabled = true;
            yaxisDropdown.enabled = true;
            zaxisDropdown.enabled = true;
            xaxisDropdown.interactable = true;
            yaxisDropdown.interactable = true;
            zaxisDropdown.interactable = true;
        }
    }
    [ContextMenu("Create a graph")]
    public void createGraph()
    {


        if (dimensionToggles[0].isOn)
        {
            gCreator.dimensions = 2;
            debugText.text += "2D";
        }
        else
        {
            gCreator.dimensions = 3;
            debugText.text += "3D";
        }
        //will need more if statements if more graph types are implemented.
        if (graphToggles[0].isOn)
        {
            gCreator.graphType = GraphCreator.GraphType.BAR;
            debugText.text += " Bar Graph";
        }
        else
        {
            gCreator.graphType = GraphCreator.GraphType.SCATTER;
            debugText.text += " Scatter Graph";
        }
        //create the graph with chosen parameters.
        gCreator.xAxis = xaxisChosen;
        gCreator.yAxis = yaxisChosen;
        gCreator.zAxis = zaxisChosen;

        //gCreator input variable required
        gCreator.inputFolderName = inputvariableChosen;

        gCreator.CreateGraph();

        //decide if we need the debug text. Set to false to disable.
        debugText.enabled = false;

        //reset to default.
        xaxisDropdown.value = 2;
        yaxisDropdown.value = 3;
        zaxisDropdown.value = 1;
        variableDropdown.value = 1;




    }
    //some default variables in case no buttons are pushed
    private void setDefaults()
    {

        graphTypeChosen = GraphCreator.GraphType.SCATTER;
        //some default axes for the axisdropdown display
        xaxisDropdown.value = 2;
        yaxisDropdown.value = 3;
        zaxisDropdown.value = 1;
        //the *axis chosen variables should show up in the inspector.
        yaxisChosen = yaxisDropdown.options[3].text;
        xaxisChosen = xaxisDropdown.options[2].text;
        zaxisChosen = zaxisDropdown.options[1].text;
        variableDropdown.value = defaultIndex;
        inputvariableChosen = variableDropdown.options[defaultIndex].text;
        dimensionChosen = 3.ToString();
        gCreator.dimensions = 3;



    }
    // Update is called once per frame
    void Update()
    {
        //graphsCreatable is set to true during initial setup.
        //check once every frame interval to see if the GraphCreator spawn circles can be selected/have space
        if (Time.frameCount % interval == 0)
        {
            //check conditions for creating a new graph. IF true make sure button is still enabled.
            if (allowGraphCreation())
            {
                //only change it if needs to be changed to improve performance
                if (!graphsCreatable)
                {
                    //re-enable the create graph button
                    createGraphButton.enabled = true;
                    graphsCreatable = !graphsCreatable;

                }
            }
            //graph creation may need to be disabled.
            else
            {
                if (graphsCreatable)
                {
                    createGraphButton.enabled = false;
                    graphsCreatable = !graphsCreatable;
                }
                //otherwise button is already disabled, and no change required.
            }
        }

    }
    //check spawn circles have free space/is selected to determine if graph can be created.
    private bool allowGraphCreation()
    {
        if (gCreator.hasFreeSpace())
            return true;
        else if (gCreator.canCreateNewGraph())
            return true;
        else
            return false;
    }

}
