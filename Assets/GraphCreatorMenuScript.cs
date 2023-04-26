using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using IATK;
using System;

public class GraphCreatorMenuScript : MonoBehaviour
{
    //references to create graph and graph config will be required
    public GraphCreator gCreator;
    //this only exists on a graph at runtime after creation, so may need another way to access the generated data sets
    public GraphConfig gConfig;
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
    public string graphTypeChosen;
    public string dimensionChosen;
    public List<Toggle> graphToggles;
    public List<Toggle> dimensionToggles;

    // Start is called before the first frame update
    void Start()
    {
        //GraphCreator has access to GraphCommon fields

        Canvas canvas = this.GetComponent<Canvas>();
        List<string> variableList = gCreator.variables;
        //axisDropdown = canvas.GetComponentInChildrenWithTag //cannot get 'WithTag' to work. Understanding needed for Heather to use.
        //TODO in future sprint - get list from available input type folders (count the CSVs)
        xaxisDropdown.options.Clear();
        yaxisDropdown.options.Clear();
        zaxisDropdown.options.Clear();

        //fill the dropdowns
        xaxisDropdown.AddOptions(variableList);
        yaxisDropdown.AddOptions(variableList);
        zaxisDropdown.AddOptions(variableList);

        //create listeners. to be tested in VR.
        xaxisDropdown.onValueChanged.AddListener(delegate { axisDropdownItemSelected(xaxisDropdown); });
        yaxisDropdown.onValueChanged.AddListener(delegate { axisDropdownItemSelected(yaxisDropdown); });
        zaxisDropdown.onValueChanged.AddListener(delegate { axisDropdownItemSelected(zaxisDropdown); });


        //populate the variable drop down menu
        variableDropdown.options.Clear();
        //we want the default_inputs chosen to start with.
        variableDropdown.AddOptions(gCreator.availableInputs);

        variableDropdown.onValueChanged.AddListener(delegate { variableDropdownItemSelected(variableDropdown); });

        debugText.text = "";
        debugText.enabled = false; //if not using the debug text.
        
        //graphToggles[0] = bar graph, graphToggles[1] = scatter graph;
        //dimensionToggles[0] = 2D, dimensionToggles[1] = 3D.
        //graphToggles[0].





    }

    private void variableDropdownItemSelected(Dropdown variableDropdown)
    {
        int index = variableDropdown.value;
        inputvariableChosen = variableDropdown.options[index].text;
        
    }

    [ContextMenu("Choose Axis")] //Heather doesn't know how to make this show up in inspector yet.
    private void axisDropdownItemSelected(Dropdown axisDropdown)
    {
        //TODO later sprint: Put the chosen axes into a list to be used for filtering the graph output
        //TODO find out if the Filtering UI will handle whether the .csv suffix is required
        int index = axisDropdown.value;
        //do something with text
        string axis = axisDropdown.options[index].text;
        //for troubleshooting.
        if (!axesChosen.Contains(axis))
            axesChosen.Add(axisDropdown.options[index].text);
        else
            axesChosen.Remove(axis);

        //TODO: test in VR if the correct axis chosen is filled with the user choice. Otherwise will need to create separate listeners. 
        if (axisDropdown.Equals(xaxisDropdown))
            xaxisChosen = axisDropdown.options[index].text;
        else if (axisDropdown.Equals(yaxisDropdown))
            yaxisChosen = axisDropdown.options[index].text;
        else
            zaxisChosen = axisDropdown.options[index].text;
        
        //TODO use some visual aid to show that axes are removed or re-added to the filter list (maybe a text box that refreshes the list
        //OR if can add a checkbox to the drop down options



    }

    public void createGraph()
    {
        //link to the graph creator
        //TODO get David's advice on whether the create graph can be set up this way.
        //can also access Graph Common
        //gCreator.
        //xaxisDropdown.selected
        //
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

        //create the graph with chosen parameters.


        //filter the graph with chosen axes


        //clear the fields OR reset to default.



    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
