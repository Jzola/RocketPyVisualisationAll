using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using IATK;

public class GraphCreatorMenuScript : MonoBehaviour
{
    //references to create graph and graph config will be required
    public GraphCreator gCreator;
    //this only exists on a graph at runtime after creation, so may need another way to access the generated data sets
    public GraphConfig gConfig;
    public Dropdown axisDropdown;
    public Dropdown variableDropdown;
    public ToggleGroup graphTypeChoice;
    public ToggleGroup dimensionsChoice;
    public Text debugText;
    public Button createGraphButton;
    public List<int> axesChosen = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = this.GetComponent<Canvas>();
        //axisDropdown = canvas.GetComponentInChildrenWithTag //cannot get 'WithTag' to work. Understanding needed for Heather to use.
        //TODO in future sprint - get list from available input type folders (count the CSVs)
        axisDropdown.options.Clear();
        //Sprint 3, just use 1-30 by default
        List<string> optionsList = new List<string>();
        for (int i = 1; i < 31; i++)
        {
            optionsList.Add("" + i);
        }
        //populate the axes dropdown 
        axisDropdown.AddOptions(optionsList);

        //create listener
        axisDropdown.onValueChanged.AddListener(delegate { axisDropdownItemSelected(axisDropdown); });


        //populate the variable drop down menu
        variableDropdown.options.Clear();
        //we want the default_inputs chosen to start with.

        //TODO check folders that are present 
        //get folders 
        string debugTextStr = "";
        string path = "Assets\\Resources\\AdditionalOutputs";
        string[] folder_paths = Directory.GetDirectories(path);

        List<string> variableOptionsList = new List<string>();
        //folder_paths = Directory.
        foreach (string folder in folder_paths)
        {
            //display without the Assets Resources path
            //
            string newfolder = folder.Replace(path+"\\", "");
            //this folder has sub folders "Env1, Env2" etc. so is not set up correctly to be automatically loaded.
            if(!newfolder.Equals("WeatherChangeMass"))
                variableOptionsList.Add(newfolder);
            Dropdown.OptionData odata = new Dropdown.OptionData();
            odata.text = newfolder;
            
            debugTextStr = debugTextStr + newfolder +"\n";
        }

        variableDropdown.AddOptions(variableOptionsList);

        debugText.text = debugTextStr;
        //debugText.enabled = false; //if not using the debug text.
        //Path.





    }
    [ContextMenu("Choose Axis")] //Heather doesn't know how to make this show up in inspector yet.
    private void axisDropdownItemSelected(Dropdown axisDropdown)
    {
        //TODO later sprint: Put the chosen axes into a list to be used for filtering the graph output
        //TODO find out if the Filtering UI will handle whether the .csv suffix is required
        int index = axisDropdown.value;
        //do something with the index
        int axis = int.Parse(axisDropdown.options[index].text);
        //if the axis is not contained in the list then add it to the list for filtering (to be used in later sprint).
        if (!axesChosen.Contains(axis))
            axesChosen.Add(int.Parse(axisDropdown.options[index].text));
        else
            axesChosen.Remove(axis);

        //TODO use some visual aid to show that axes are removed or re-added to the filter list (maybe a text box that refreshes the list
        //OR if can add a checkbox to the drop down options



    }

    public void createGraph()
    {
        //link to the graph creator
        //TODO get David's advice on whether the create graph can be set up this way.
        //gCreator.variables.
        //gCreator.dimensions = 

        //create the graph with chosen parameters.


        //filter the graph with chosen axes


        //clear the fields OR reset to default.



    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
