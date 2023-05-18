using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class GraphConfigMenuScript : MonoBehaviour
{
    private GraphConfig gConfig;
    private OutputVariableVisibility outputConfig;

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
    public Button toggleDataDisplayVisibility;
    private int inputVarIndex;
    private int xAxisIndex=2;
    private int yAxisIndex = 3;
    private int zAxisIndex = 1;
    //private List<Toggle> dataToggles;
    public int focusIDSlider= -1;
    public GameObject dataScrollViewContent;
    private int trajectoriesMax = 29;
    public GameObject toggleTemplate;
    public int visIndex=0;
    public List<GameObject> dataToggles;
    

    // Start is called before the first frame update
    void Start()
    {
        //initialize the graph config and text fields
        gConfig = transform.parent.gameObject.GetComponent<GraphConfig>();
        outputConfig = transform.parent.gameObject.GetComponent<OutputVariableVisibility>();

        fIDText.text = gConfig.focusID;
        fTypeText.text = gConfig.focusType;
        fValueText.text = gConfig.focusValue;
        fEngineText.text = gConfig.focusEngine;


        setupDataDisplay();

        //get variables and axes already sent from graph creation
        if (gConfig.inputFolderName != null) {
           inputVarIndex = gConfig.availableInputs.IndexOf(gConfig.inputFolderName);
        }
        else
        {
            inputVarIndex = 2;
        }
        //bring over the axis settings from graph creator, or use default values.
        if(gConfig.xAxis != null){
            xAxisIndex = gConfig.variables.IndexOf(gConfig.xAxis);
        }

        if(gConfig.yAxis != null)
        {
            yAxisIndex = gConfig.variables.IndexOf(gConfig.yAxis);
        }
        if(gConfig.zAxis != null)
        {
            zAxisIndex = gConfig.variables.IndexOf(gConfig.zAxis);
        }
        //set up the options in the dropdowns, and put the selected value at the default or the one chosen in graph creation
        setUpDropdown(xAxisDropdown, gConfig.variables, xAxisIndex);
        setUpDropdown(yAxisDropdown, gConfig.variables, yAxisIndex);
        setUpDropdown(zAxisDropdown, gConfig.variables, zAxisIndex);
        setUpDropdown(inputDropdown, gConfig.availableInputs, inputVarIndex);


        if (gConfig.dimensions == 2)
        {
            //remove the z axis dropdown if 2d graph.
            zAxisDropdown.gameObject.SetActive(false);
        }

        //currently the slider uses the max trajecjectories = 30, but we include "1.csv" as 0.
        SetupSlider(trajectorySlider, 29);
        //avoid errors from no fields chosen
        setDefaults();

        updateGraphButton.onClick.AddListener(applyGraphChanges);

    }
    private void toggleDataDisplayController()
    {
        //get canvas and either scale down or scale up
    }
    //use this if we get time to 
    private void setupDataDisplay()
    {
        //get the number of possible output variables from graph config. Default as of May 2023 is 49 variables
        int noToggles = gConfig.variables.Count;
        //get the scrollview. The canvas is a direct child of the menu object
        //GameObject content = gameObject.GetComponentsInChildren<Content>
        int togPosition = 0;
       // dataToggles.Add(toggle);

        //add the toggles to the data panel and connect them to a listener that ajdusts variable visibility
        foreach (string vars in gConfig.variables)
        {
            //may need to instantiate first
            //Toggle toggle = dataScrollViewContent.AddComponent<Toggle>();
            int index = gConfig.variables.IndexOf(vars);
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

    public void outputVisibilityListener(Toggle tog, int index)
    {

        outputConfig.setVisibility(index, tog.isOn);
    }
    [ContextMenu("Test visibility")]
    public void outputVisibilityTester()
    {
        //use the index to get the children of the content box
        //visIndex;
        dataToggles[visIndex].GetComponent<Toggle>().isOn = !dataToggles[visIndex].GetComponent<Toggle>().isOn;
        
    }
    private void updateSliderValues()
    {
        //
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
        //value can be changed manually in inspector mode
        value = focusIDSlider;
        //changing the value should automatically update the listener
        trajectorySlider.value = value;


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

        //reset UI

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
