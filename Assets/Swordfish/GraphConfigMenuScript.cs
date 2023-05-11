using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class GraphConfigMenuScript : MonoBehaviour
{
    private GraphConfig gConfig;
    //variables taken from this graph's graph config.
    //private string focusID;
    //private string focusType;
    //private string focusValue;
    public Slider trajectorySlider;
    public Text fIDText;
    public Text fTypeText;
    public Text fValueText;
    public Text fEngineText;
    public Dropdown xAxisDropdown;
    public Dropdown yAxisDropdown;
    public Dropdown zAxisDropdown;
    public Dropdown inputDropdown;
    public Button updateGraphButton;
    private int inputVarIndex;
    private int xAxisIndex;
    private int yAxisIndex = 3;
    private int zAxisIndex = 1;

    public int focusIDSlider= -1;
    private int trajectoriesMax = 29;

    // Start is called before the first frame update
    void Start()
    {
        //initialize the graph config and text fields
        gConfig = transform.parent.gameObject.GetComponent<GraphConfig>();
        
        fIDText.text = gConfig.focusID;
        fTypeText.text = gConfig.focusType;
        fValueText.text = gConfig.focusValue;
        fEngineText.text = gConfig.focusEngine;

       


        //get variables and axes already sent from graph creation
        if (gConfig.inputFolderName != null) {
           inputVarIndex = gConfig.availableInputs.IndexOf(gConfig.inputFolderName);
        }
        else
        {
            inputVarIndex = 2;
        }

        if(gConfig.xAxis != null){
            xAxisIndex = gConfig.variables.IndexOf(gConfig.xAxis); }
        else
        {
            xAxisIndex = 2;
        }
        if(gConfig.yAxis != null)
        {
            yAxisIndex = gConfig.variables.IndexOf(gConfig.yAxis);
        }
        if(gConfig.zAxis != null)
        {
            zAxisIndex = gConfig.variables.IndexOf(gConfig.zAxis);
        }
        //
        setUpDropdown(xAxisDropdown, gConfig.variables, xAxisIndex);
        setUpDropdown(yAxisDropdown, gConfig.variables, yAxisIndex);
        setUpDropdown(zAxisDropdown, gConfig.variables, zAxisIndex);
        setUpDropdown(inputDropdown, gConfig.availableInputs, inputVarIndex);


        if (gConfig.dimensions == 2)
        {
            //remove the z axis dropdown if 2d graph.
            zAxisDropdown.gameObject.SetActive(false);
        }

        //currently the slider uses the max trajecjectories = 30, but we include 0.
        SetupSlider(trajectorySlider, 29);
        //avoid errors from no fields chosen
        setDefaults();

        updateGraphButton.onClick.AddListener(applyGraphChanges);

    }
    private void updateSliderValues()
    {
        //to be tested in VR
        int value = (int)trajectorySlider.value;
        //
        gConfig.selectTrajectory(value);
        //update the focus 
        fIDText.text = gConfig.focusID;
        fTypeText.text = gConfig.focusType;
        fValueText.text = gConfig.focusValue;
        fEngineText.text = gConfig.focusEngine;


    }
    [ContextMenu("Slider Testing")]
    public void updateSliderValuesTesting()
    {
        int value = 0;
        if (focusIDSlider > trajectoriesMax)
            value = 29;
        //to be tested in Desktop mode
        value = focusIDSlider;
        trajectorySlider.value = value; //should automatically update the listener
       

    }
    private void updateFocusText()
    {
        fIDText.text = gConfig.focusID;
        fTypeText.text = gConfig.focusType;
        fValueText.text = gConfig.focusValue;
        fEngineText.text = gConfig.focusEngine;
    }
    private void SetupSlider(Slider slider, int valRange)
    {
        //

        slider.minValue = -1;
        slider.maxValue = valRange;
        slider.wholeNumbers = true;
        slider.value = -1;
        slider.onValueChanged.AddListener(delegate { updateSliderValues(); });
    }
    private void setupTextFields(string fID, string fType, string fValue)
    {

    }
    //temporarily disable all UI components
    private void setAllUIInactive()
    {
        trajectorySlider.enabled = false;
        xAxisDropdown.enabled = false;
        yAxisDropdown.enabled = false;
        zAxisDropdown.enabled = false;
        inputDropdown.enabled = false;

    }

    //re-enable all UI components
    private void setAllUIActive()
    {
        trajectorySlider.enabled = true;
        xAxisDropdown.enabled = true;
        yAxisDropdown.enabled = true;
        zAxisDropdown.enabled = true;
        inputDropdown.enabled = true;

    }

    private void applyGraphChanges()
    {
        setAllUIInactive();

        gConfig.UpdateGraph();

        //reset text.
        updateFocusText();

        //reset 

        setAllUIActive();
    }

    private void setUpDropdown(Dropdown dDown, List<string> options, int defaultIndex)
    {
        dDown.options.Clear();
        dDown.AddOptions(options);
        dDown.value = defaultIndex;
        dDown.onValueChanged.AddListener(delegate { dropdownItemSelected(dDown); });

    }

    private void setDefaults()
    {
        //set up the graphConfig
        int index = xAxisDropdown.value;
        
        gConfig.xAxis = xAxisDropdown.options[index].text;
        index = yAxisDropdown.value;
        gConfig.yAxis = yAxisDropdown.options[index].text;
        index = zAxisDropdown.value;
        gConfig.zAxis = zAxisDropdown.options[index].text;

        index = inputDropdown.value;
        gConfig.inputFolderName = inputDropdown.options[index].text;
        
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void dropdownItemSelected(Dropdown axisDropdown)
    {

        int index = axisDropdown.value;
        //do something with text
        string axis = axisDropdown.options[index].text;

        //TODO: test in VR if the correct axis chosen is filled with the user choice. Otherwise will need to create separate listeners. 
        if (axisDropdown.Equals(xAxisDropdown))
        {

            gConfig.xAxis = axisDropdown.options[index].text;

        }

        else if (axisDropdown.Equals(yAxisDropdown))
        {
            gConfig.yAxis = axisDropdown.options[index].text;


        }

        else if (axisDropdown.Equals(zAxisDropdown))
        {
            gConfig.zAxis = axisDropdown.options[index].text;

        }
        else if (axisDropdown.Equals(inputDropdown))
        {
            gConfig.inputFolderName = axisDropdown.options[index].text;
        }
    }
}
