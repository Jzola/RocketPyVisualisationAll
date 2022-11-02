using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BarGraph.VittorCloud;
using System.Linq;
using UnityEngine.Events;

public class ThirdLevelChartLinkingManager : ChartLinkingManager
{
    // UI Elements
    public Slider xSlider;
    public Slider zSlider;
    public Slider singleSlider;
    private bool zHidden = false;
    private bool xHidden = false;
   
    // Bar Graph Properties
    private bool init = false;
    private BarGraphManager barManager;
    private List<BarGroup> barGroups;
    private List<GameObject> allBarsGOs;

    // ID's of the currently highlighted trajectories/bars
    private List<int> xSelected;
    private List<int> zSelected;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        barManager = GetComponentInChildren<BarGraphManager>();
        highlightingMaterial = Resources.Load<Material>("Materials/GreenHighlighting");

        xSelected = new List<int>();
        zSelected = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            if (barManager != null)
            {
                barGroups = barManager.ListOfGroups;

                // Z axis
                SetupSlider(zSlider, barGroups.Count, ZSliderChanged);

                // X axis 
                int xCount = 0;
                foreach (BarGroup group in barGroups)
                {
                    if (group.ListOfBar.Count > xCount)
                        xCount = group.ListOfBar.Count;
                }
                SetupSlider(xSlider, xCount, XSliderChanged);

                // Single bar highlighting slider
                SetupSlider(singleSlider, (xCount * barManager.ListOfGroups.Count), SingleSliderChanged);
                // Hide the single slider 
                singleSlider.transform.parent.gameObject.SetActive(false);
                SetHeightOfParentRectTransform(208.9f);

                // Hide highlighting UI on start                      
                xSlider.transform.parent.parent.gameObject.SetActive(false);

                // Get all bars in the chart to iterate over for single bar highlighting
                allBarsGOs = new List<GameObject>();
                foreach (BarGroup group in barGroups)
                {
                    allBarsGOs.AddRange(group.ListOfBar);
                }

                init = true;
            } else
            {
                barManager = GetComponentInChildren<BarGraphManager>();
            }
        }
    }

    // Highlights a row of bars
    private void HighlightZBars(int index)
    {
        if (xHidden)
        {
            HideTrajectories();
        }
        else
        {
            HideTrajectories(zSelected.Except(xSelected).ToList<int>());
        }

        zSelected = new List<int>();
        for (int i = 0; i < barGroups.Count; i++)
        {
            List<GameObject> bars = barGroups[i].ListOfBar;
            for (int j = 0; j < bars.Count; j++)
            {
                if (i == index)
                {
                    bars[j].GetComponentInChildren<MeshRenderer>().material = highlightingMaterial;
                    bars[j].GetComponentInChildren<BarProperty>().LabelContainer.SetActive(true);
                    zSelected.Add(bars[j].GetComponent<BarProperty>().GetBarId());
                    // Set slider text to show Z axis value
                    zSlider.GetComponentInChildren<Text>().text = bars[j].GetComponentInChildren<Bar>().GetZValue();                    
                }
                else if (!xSelected.Contains(bars[j].GetComponent<BarProperty>().GetBarId()) || xHidden)
                {
                    bars[j].GetComponent<BarProperty>().ResetBarMatToOriginal();
                    bars[j].GetComponentInChildren<BarProperty>().LabelContainer.SetActive(false);
                }
            }
        }
        HighlightTrajectories(zSelected);
    }

    // Highlights a column of bars
    private void HighlightXBars(int index)
    {
        if (zHidden)
        {
            HideTrajectories();
        }
        else
        {
            HideTrajectories(xSelected.Except(zSelected).ToList<int>());
        }

        xSelected = new List<int>();
        for (int i = 0; i < barGroups.Count; i++)
        {
            List<GameObject> bars = barGroups[i].ListOfBar;
            for (int j = 0; j < bars.Count; j++)
            {
                if (j == index)
                {
                    bars[j].GetComponentInChildren<MeshRenderer>().material = highlightingMaterial;
                    bars[j].GetComponentInChildren<BarProperty>().LabelContainer.SetActive(true);
                    xSelected.Add(bars[j].GetComponent<BarProperty>().GetBarId());
                    // Set slider text to show X axis value
                    xSlider.GetComponentInChildren<Text>().text = bars[j].GetComponentInChildren<Bar>().GetXValue();
                }
                else if (!zSelected.Contains(bars[j].GetComponent<BarProperty>().GetBarId()) || zHidden)
                {
                    bars[j].GetComponent<BarProperty>().ResetBarMatToOriginal();
                    bars[j].GetComponentInChildren<BarProperty>().LabelContainer.SetActive(false);
                }
            }
        }
        HighlightTrajectories(xSelected);
    }

    // If false, un-highlight all currently selected z-axis bars and lock the slider
    public void ShowZSlider(bool isOn)
    {
        if (isOn == false)
        {
            List<int> zAxis = zSelected.Except(xSelected).ToList<int>();
            HideTrajectories(zAxis);
            ResetBarsToOriginal(zAxis);
            zSlider.enabled = false;
            zHidden = true;
            zSlider.GetComponentInChildren<Text>().text = "";
        }
        // un-lock z-slider
        else
        {
            zSlider.enabled = true;
            zHidden = false;
            HighlightZBars((int)zSlider.value);
        }

        if (zHidden && xHidden)
        {
            singleSlider.transform.parent.gameObject.SetActive(true);
            SetHeightOfParentRectTransform(312.97f);
            SingleSliderChanged(singleSlider.value);
        } else
        {
            singleSlider.transform.parent.gameObject.SetActive(false);
            SetHeightOfParentRectTransform(208.9f);
        }
    }

    // If false, un-highlight all currently selected x-axis bars and lock the slider
    public void ShowXSlider(bool isOn)
    {
        if (isOn == false)
        {
            List<int> xAxis = xSelected.Except(zSelected).ToList<int>();
            HideTrajectories(xAxis);
            ResetBarsToOriginal(xAxis);
            xSlider.enabled = false;
            xHidden = true;
            xSlider.GetComponentInChildren<Text>().text = "";
        }
        // un-lock x-slider
        else
        {
            xSlider.enabled = true;
            xHidden = false;
            HighlightXBars((int)xSlider.value);
        }

        if (zHidden && xHidden)
        {
            singleSlider.transform.parent.gameObject.SetActive(true);
            SetHeightOfParentRectTransform(312.97f);
            SingleSliderChanged(singleSlider.value);
        } else
        {
            singleSlider.transform.parent.gameObject.SetActive(false);
            SetHeightOfParentRectTransform(208.9f);


            // Change single value slider text to empty
            singleSlider.GetComponentInChildren<Text>().text = "";
        }
    }

    // Resets a list of bars of bar chart to their original material
    protected void ResetBarsToOriginal(List<int> ids)
    {
        for (int i = 0; i < barGroups.Count; i++)
        {
            List<GameObject> bars = barGroups[i].ListOfBar;
            for (int j = 0; j < bars.Count; j++)
            {
                int id = bars[j].GetComponent<BarProperty>().GetBarId();

                if (ids.Contains(id))
                {
                    bars[j].GetComponent<BarProperty>().ResetBarMatToOriginal();
                    bars[j].GetComponentInChildren<BarProperty>().LabelContainer.SetActive(false);
                }
            }
        }
    }

    private void ZSliderChanged(float value)
    {
        HighlightZBars((int)value);
    }

    private void XSliderChanged(float value)
    {
        HighlightXBars((int)value);
    }

    private void SingleSliderChanged(float value)
    {
        GameObject barObj = allBarsGOs[(int)value];
        Bar bar = barObj.GetComponentInChildren<Bar>();
        singleSlider.GetComponentInChildren<Text>().text = bar.GetZValue() + " + " + bar.GetXValue();
        xSlider.GetComponentInChildren<Text>().text = "";
        zSlider.GetComponentInChildren<Text>().text = "";

        int barID = barObj.GetComponent<BarProperty>().GetBarId();
        ResetBarsToOriginal();
        HighlightBar(barID);
        HighlightTrajectory(barID);
    }

    private void SetupSlider(Slider slider, int valRange, UnityAction<float> action)
    {
        // Convert valRange from int to a list of floats to one decimal place 
        List<float> sliderValues = Enumerable.Range(0, valRange).Select(i => i / 1F).ToList();
        slider.GetComponent<UISliderStep>().SetSliderValues(sliderValues);
        slider.wholeNumbers = true;
        slider.onValueChanged.AddListener(action);
    }

    // UI helper functions
    // Resize the parent rect (with white background) dynamically as the single slider is being shown/hidden
    private void SetHeightOfParentRectTransform(float height)
    {
        RectTransform rt = singleSlider.transform.parent.GetComponentInParent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
    }

    public void ShowHighlightingUI(bool isOn)
    {
        doHighlighting = isOn;
        if (isOn)
        {
            xSlider.transform.parent.parent.gameObject.SetActive(true);
            HideTrajectories();
            if (!xHidden)
                XSliderChanged((int)xSlider.value);
            if (!zHidden)
                ZSliderChanged((int)zSlider.value);
            else if (zHidden && xHidden)
                SingleSliderChanged((int)singleSlider.value);
        }
        else
        {
            xSlider.transform.parent.parent.gameObject.SetActive(false);
            ResetBarsToOriginal();
            ResetTrajectoriesToOriginal();
        }
    }

    // Called when user hovers over an X axis label on the bar graph
    // Highlights the entire corresponding X axis of bars
    public void BarXLabelHover(int index)
    {
        if (doHighlighting && !xHidden)
        {
            HighlightXBars(index);
            // Change slider value accordingly
            xSlider.value = index;
        }
    }


    // Called when user hovers over an Z axis label on the bar graph
    // Highlights the entire corresponding Z axis of bars
    public void BarZLabelHover(int index)
    {
        if (doHighlighting && !zHidden)
        {
            HighlightZBars(index);
            // Change slider value accordingly
            zSlider.value = index;
        }
    }
}