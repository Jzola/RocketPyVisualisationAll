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

        //create listeners. 
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

        
        //add a listener to the graph dimensions options/toggles that disables z axis if 2D is checked, and enables if 3D is checked.
        
        dimensionToggles[0].onValueChanged.AddListener(dimensionChanged);

        setDefaults();

        //connect the button to the createGraph method
        createGraphButton.onClick.AddListener(createGraph);
        
        
       //debugText.enabled=true;
    



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
            //due to toggle group rules, this should automatically select the other toggle, but just in case.
            graphToggles[1].isOn = true;
            
            //graphTypeChoice.allowSwitchOff = false;
        }
        else
        {
            graphToggles[1].isOn = false;
            graphToggles[0].isOn = true;
            
            
        }
        graphTypeChoice.allowSwitchOff = false;
    }

    [ContextMenu("Chexk axis removed")]
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

    //to be moved
    //[ContextMenu("Toggle Menu")]
    private void toggleMenuVisibility()
    {
        RectTransform rt;
        //this gets the shader/rendering for the anchor. Don't use this With maximizer canvas.
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
            

        //show that axes are added (optional)
        debugText.enabled = false;


    }
    [ContextMenu("Create a graph")]
    public void createGraph()
    {

        //can also access Graph Common for variables

        //test z axis disappears
        //dimensionToggles[0].isOn= true;
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
        //test code
        //dimensionChanged(true);



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
