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
    public GraphCreator.GraphType graphTypeChosen;
    public string dimensionChosen;
    public List<Toggle> graphToggles;
    public List<Toggle> dimensionToggles;
    private MeshRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        //GraphCreator has access to GraphCommon fields

        Canvas canvas = this.GetComponent<Canvas>();
        //get the anchor and make it invisible to start (make it visible when the menu is minimised)
        GameObject anchor = this.transform.parent.gameObject;
        rend = anchor.GetComponent<MeshRenderer>();
        rend.enabled = false;
        //anchor.GetComponent<Renderer>.enabled = false;
        //anchor.transform.localScale = new Vector3(0, 0, 0);
        List<string> variableList = gCreator.variables;
        //axisDropdown = canvas.GetComponentInChildrenWithTag //cannot get 'WithTag' to work. Understanding needed for Heather to use.
        //
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
        debugText.enabled = false; //if not using the debug text, set to false.

        //graphToggles[0] = bar graph, graphToggles[1] = scatter graph;
        //dimensionToggles[0] = 2D, dimensionToggles[1] = 3D.
        //can either add a listener here  or handle checking in the createGraph method

        //set defaults (will also prevent errors if create graph is clicked before any changes made).
        //add a listener to the graph dimensions options/toggles that disables z axis if 2D is checked, and enables if 3D is checked.
        //dimensionToggles[0].OnSelect.AddListener(toggleMenuVisibility);
        dimensionToggles[0].onValueChanged.AddListener(dimensionChanged);

        setDefaults();

        //connect the button to the createGraph method
        createGraphButton.onClick.AddListener(createGraph);
        //these listeners need to be tested in VR. //DOESN'T WORK.
        anchor.GetComponent<Button>().onClick.AddListener(toggleMenuVisibility);
        Button btnName = anchor.GetComponent<Button>();
        debugText.text = "Check anchor " + btnName.name;
        
        Button minButton = anchor.GetComponentInChildrenWithTag<Button>("Minimize");
        
        Debug.Log("min button" + minButton.name); //too many warnings

        debugText.text += " min button" + minButton.name;
        ///debugText.enabled=true;
        minButton.onClick.AddListener(toggleMenuVisibility);



    }
    [ContextMenu("Change dimension")]
    private void dimensionChanged(bool arg0)
    {
        //test code
        dimensionToggles[0].isOn = true;
        if (dimensionToggles[0].isOn)
        {
            //the 2D option has been selected, so there is no Z axis
            //should I be using isActiveAndEnabled?
            zaxisDropdown.enabled = false;
        }
        else
        {
            zaxisDropdown.enabled = true;
        }
        //throw new NotImplementedException();
    }

    //to be moved
    [ContextMenu("Toggle Menu")]
    private void toggleMenuVisibility()
    {
        RectTransform rt;
        //this gets the shader/rendering for the anchor. Don't use this. With maximizer canvas.
        rt = GetComponent<RectTransform>();
        if (rend.enabled == false)
        {
            rend.enabled = true;
            this.GetComponent<CanvasGroup>().alpha = 1;
            rt = GetComponent<RectTransform>();
            rt.localScale = new Vector3(0, 0, 0);

        }
        else
        {
            rend.enabled = false;
            //replace this with a variable taken from the menu's original size, not a hard coded value.
            rt.localScale= new Vector3((float)0.0050148922, (float)0.00701489206, (float)0.0050148922);
            //Vector3(0.0050148922,0.00701489206,0.0050148922)
            this.GetComponent<CanvasGroup>().alpha = 1;
        }

    }

    private void variableDropdownItemSelected(Dropdown variableDropdown)
    {
        int index = variableDropdown.value;
        inputvariableChosen = variableDropdown.options[index].text;
        
    }

    [ContextMenu("Choose Axis")] //Heather doesn't know how to make this show up in inspector yet.
    private void axisDropdownItemSelected(Dropdown axisDropdown)
    {
        
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
            

        //show that axes are added
        debugText.enabled = false;


    }
    [ContextMenu("Create a graph")]
    public void createGraph()
    {
        //link to the graph creator
        //TODO get David's advice on whether the create graph can be set up this way.
        //can also access Graph Common for variables

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

        //needs to be tested in VR
        gCreator.CreateGraph();

        //decide if we need the debug text. Set to false to disable.
        debugText.enabled = true;

        //clear the fields OR reset to default.
        xaxisDropdown.value = 2;
        yaxisDropdown.value = 3;
        zaxisDropdown.value = 1;
        variableDropdown.value = 2;



    }
    //some default variables in case no buttons are pushed
    private void setDefaults()
    {
        graphTypeChosen = GraphCreator.GraphType.SCATTER;
        xaxisChosen = xaxisDropdown.options[2].text;
        //some default axes for the axisdropdown display
        xaxisDropdown.value = 2;
        yaxisDropdown.value = 3;
        zaxisDropdown.value = 1;
        //the *axis chosen variables should show up in the inspector.
        yaxisChosen = yaxisDropdown.options[3].text;
        zaxisChosen = zaxisDropdown.options[1].text;
        variableDropdown.value = 2;
        inputvariableChosen = variableDropdown.options[2].text;
        dimensionChosen = 3.ToString();
        gCreator.dimensions = 3;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
