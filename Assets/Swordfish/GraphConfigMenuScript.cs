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
    private string focusID;
    private string focusType;
    private string focusValue;
    public Slider trajectorySlider;
    public Text fIDText;
    public Text fTypeText;
    public Text fValueText;
    public Dropdown xAxisDropdown;
    public Dropdown yAxisDropdown;
    public Dropdown zAxisDropdown;
    public Dropdown inputDropdown;

    // Start is called before the first frame update
    void Start()
    {
        //initialize the graph config and text fields
        gConfig = transform.parent.gameObject.GetComponent<GraphConfig>();
        focusID = gConfig.focusID;
        focusType = gConfig.focusType;
        focusValue = gConfig.focusValue;

        fIDText.text = focusID;
        fTypeText.text = focusType;
        fValueText.text = focusValue;


        setUpDropdown(xAxisDropdown, gConfig.variables, 2);
        setUpDropdown(yAxisDropdown, gConfig.variables, 3);
        setUpDropdown(zAxisDropdown, gConfig.variables, 1);
        setUpDropdown(inputDropdown, gConfig.availableInputs, 2);


        if (gConfig.dimensions == 2)
        {
            //remove the z axis dropdown if 2d graph.
            zAxisDropdown.gameObject.SetActive(false);
        }
        
        SetupSlider(trajectorySlider, 30, updateSliderValues);

  

    }
    private void updateSliderValues(float value)
    {

    }
    private void SetupSlider(Slider slider, int valRange, UnityAction<float> action)
    {
        // Convert valRange from int to a list of floats to one decimal place 
        List<float> sliderValues = Enumerable.Range(0, valRange).Select(i => i / 1F).ToList();
        slider.GetComponent<UISliderStep>().SetSliderValues(sliderValues);
        slider.wholeNumbers = true;
        slider.onValueChanged.AddListener(action);
    }
    private void setupTextFields(string fID, string fType, string fValue) 
    {

    }
    //temporarily disable all UI components
    private void setAllUIInactive()
    {
        trajectorySlider.enabled = false;

    }

    //re-enable all UI components
    private void setAllUIActive()
    {
        trajectorySlider.enabled = true;

    }

    private void applyGraphChanges()
    {
        setAllUIInactive();

        gConfig.UpdateGraph();

        setAllUIActive();
    }

    private void setUpDropdown(Dropdown dDown, List<string> options, int defaultIndex)
    {
        dDown.options.Clear();
        dDown.AddOptions(options);
        dDown.value = defaultIndex;

    }

    private void setDefaults()
    {
        //reset dropdowns to defualt values
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
