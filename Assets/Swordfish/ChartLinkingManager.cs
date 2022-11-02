using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using IATK;
using UnityEngine.UI;
using BarGraph.VittorCloud;

public class ChartLinkingManager : MonoBehaviour
{
    public Slider slider;

    private Visualisation trajectoryVisualisation;

    private List<CSVDataSource> trajectoryDataSources = new List<CSVDataSource>();

    protected Material highlightingMaterial;
    private Material hiddenMaterial;

    protected bool doHighlighting;
    // Whether highlighting is locked onto selected bar
    private int? lockedHighlightedID;
    private int? currentHighlightedID;

    private BarProperty[] barProperties;
    private LoadInputVariables inputVarLoader;
    private List<CSVDataSource> inputVarDataSource;

    protected virtual void Start()
    {
        trajectoryVisualisation = GetComponentsInChildren<Visualisation>()[0];

        // Get the input data source
        inputVarLoader = GetComponentInChildren<LoadInputVariables>();
        inputVarDataSource = inputVarLoader.GetDataSource();

        // Material to highlight and hide components
        highlightingMaterial = Resources.Load<Material>("Materials/RedHighlighting");
        hiddenMaterial = Resources.Load<Material>("Materials/Hidden");

        // If two dimensions
        if (inputVarLoader.axisZ == "")
        {
            // UI Discrete Slider
            slider.onValueChanged.AddListener(HighlightSliderChanged);
            // Set the Slider value ranges to range of input values
            float[] rawDimension = inputVarDataSource[0].GetCol(inputVarDataSource[0].dataArray, inputVarDataSource[0].findCol(inputVarLoader.axisX));
            slider.GetComponent<UISliderStep>().SetSliderValues(new List<float>(rawDimension));
            slider.enabled = false;
            slider.GetComponentInChildren<Text>().text = "";
        }

        // Corresponds with UI toggle being Is Off
        doHighlighting = false;

        lockedHighlightedID = null;

        // Array of individual bars within bar chart
        barProperties = GetComponentsInChildren<BarProperty>();
    }

    private int FindIdOfValue(float value)
    {
        int valueID = 0;

        // Get the column of data of the variable of interest from the input variable data source
        float[] rawDimension = inputVarDataSource[0].GetCol(inputVarDataSource[0].dataArray, inputVarDataSource[0].findCol(inputVarLoader.axisX));

        for (var i = 0; i < rawDimension.Length; i++)
        {
            if (rawDimension[i] == value)
            {
                valueID = (int)inputVarDataSource[0].GetCol(inputVarDataSource[0].dataArray, inputVarDataSource[0].findCol("id"))[i];
            }
        }
        return valueID;
    }

    private float FindValueOfId(int id)
    {
        float value = 0f;
        float[] rawDimension = inputVarDataSource[0].GetCol(inputVarDataSource[0].dataArray, inputVarDataSource[0].findCol("id"));

        for (var i = 0; i < rawDimension.Length; i++)
        {
            if (rawDimension[i] == id)
            {
                value = inputVarDataSource[0].GetCol(inputVarDataSource[0].dataArray, inputVarDataSource[0].findCol(inputVarLoader.axisX))[i];
            }
        }
        return value;
    }
    protected void HighlightTrajectory(int trajectoryID)
    {
        // If trajectory data sources have not been initialised. Cannot be in Start as data sources are generated after Start occurs.
        if (trajectoryDataSources.Count == 0)
            trajectoryDataSources = trajectoryVisualisation.GetComponentInChildren<DataFiles>().GetFiles();

        for (int i = 0; i < trajectoryDataSources.Count; i++)
        {
            // Get name of trajectory data file, which is set as it's own trajectory ID
            int fileNum = Int32.Parse(trajectoryDataSources[i].data.name);

            // Change colour of data points and line renderer of each trajectory
            DataPoint[] dataPoints = trajectoryDataSources[i].GetComponentsInChildren<DataPoint>();
            foreach (DataPoint dp in dataPoints)
            {
                // Highlight the trajectory with the corresponding trajectory ID based on file name
                if (fileNum == trajectoryID)
                    dp.ResetMaterial();
                else
                    dp.GetComponent<MeshRenderer>().material = hiddenMaterial;
            }
            VisualisationLine lr = trajectoryDataSources[i].GetComponentInChildren<VisualisationLine>();
            if (lr)
            {
                if (fileNum == trajectoryID)
                    lr.resetLineMaterialToOriginal();
                else
                    lr.GetComponent<LineRenderer>().material = hiddenMaterial;
            }
        }
    }

    // Highlight multiple trajectories at the same time
    public void HighlightTrajectories(List<int> ids)
    {
        // If trajectory data sources have not been initialised. Cannot be in Start as data sources are generated after Start occurs.
        if (trajectoryDataSources.Count == 0)
            trajectoryDataSources = trajectoryVisualisation.GetComponentInChildren<DataFiles>().GetFiles();

        for (int i = 0; i < trajectoryDataSources.Count; i++)
        {
            // Get name of trajectory data file, which is set as it's own trajectory ID
            int fileNum = Int32.Parse(trajectoryDataSources[i].data.name);

            // Change colour of data points and line renderer of each trajectory
            MeshRenderer[] meshRenderers = trajectoryDataSources[i].GetComponentsInChildren<MeshRenderer>();
            DataPoint[] dps = trajectoryDataSources[i].GetComponentsInChildren<DataPoint>();
            foreach (DataPoint dp in dps)
            {
                // Highlight the trajectory with the corresponding trajectory ID based on file name
                if (ids.Contains(fileNum))
                    dp.ResetMaterial();
            }
           
            VisualisationLine lr = trajectoryDataSources[i].GetComponentInChildren<VisualisationLine>();
            if (lr)
            {
                if (ids.Contains(fileNum))
                    lr.resetLineMaterialToOriginal();
            }
        }
    }

    public void HideTrajectories(List<int> ids)
    {
        // If trajectory data sources have not been initialised. Cannot be in Start as data sources are generated after Start occurs.
        if (trajectoryDataSources.Count == 0)
            trajectoryDataSources = trajectoryVisualisation.GetComponentInChildren<DataFiles>().GetFiles();

        for (int i = 0; i < trajectoryDataSources.Count; i++)
        {
            // Get name of trajectory data file, which is set as it's own trajectory ID
            int fileNum = Int32.Parse(trajectoryDataSources[i].data.name);

            // Change colour of data points and line renderer of each trajectory
            MeshRenderer[] meshRenderers = trajectoryDataSources[i].GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in meshRenderers)
            {
                // Highlight the trajectory with the corresponding trajectory ID based on file name
                if (ids.Contains(fileNum))
                    mr.material = hiddenMaterial;
            }
            LineRenderer lr = trajectoryDataSources[i].GetComponentInChildren<LineRenderer>();
            if (lr)
            {
                if (ids.Contains(fileNum))
                    lr.material = hiddenMaterial;
            }
        }
    }

    // Hides ALL trajectories
    public void HideTrajectories()
    {
        // If trajectory data sources have not been initialised. Cannot be in Start as data sources are generated after Start occurs.
        if (trajectoryDataSources.Count == 0)
            trajectoryDataSources = trajectoryVisualisation.GetComponentInChildren<DataFiles>().GetFiles();

        for (int i = 0; i < trajectoryDataSources.Count; i++)
        {
            // Change colour of data points and line renderer of each trajectory
            MeshRenderer[] meshRenderers = trajectoryDataSources[i].GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in meshRenderers)
            {
                mr.material = hiddenMaterial;
            }
            LineRenderer lr = trajectoryDataSources[i].GetComponentInChildren<LineRenderer>();
            if (lr)
            {
                lr.material = hiddenMaterial;
            }
        }
    }

    // Highlight a single bar within bar chart
    protected void HighlightBar(int trajectoryID)
    {          
        foreach (BarProperty bar in barProperties)
        {
            if (bar.GetBarId() == trajectoryID)
            {
                bar.SetBarMat(highlightingMaterial);
                bar.LabelContainer.SetActive(true);
                // Set text of slider to bar value
                if(slider)
                    slider.GetComponentInChildren<Text>().text = bar.GetComponentInChildren<Bar>().GetXValue();
            }
            else
            {
                bar.SetBarMat(hiddenMaterial);
                bar.LabelContainer.SetActive(false);
            }
        }
    }
    
    // Main function that does that higlighting across bar, trajectory and move slider
    public void HighlightID(int trajectoryID)
    {
        if (currentHighlightedID != trajectoryID)
        {
            if (doHighlighting && lockedHighlightedID == null)
            {
                HighlightBar(trajectoryID);
                HighlightTrajectory(trajectoryID);
                if (slider)
                    slider.value = FindValueOfId(trajectoryID);
                currentHighlightedID = trajectoryID;
            }
        }        
    }

    // Lock highlighting to a single trajectory/bar (Only works with 2D bar chart)
    public void LockHighlighting(int trajectoryID)
    {
        if (lockedHighlightedID == trajectoryID)
        {
            lockedHighlightedID = null;
        } else
        {
            lockedHighlightedID = trajectoryID;
        }
    }

    public void UnlockHighlightedTrajectory()
    {
        lockedHighlightedID = null;
    }

    // Listener function for when slider value has changed
    private void HighlightSliderChanged(float value)
    {
        if (doHighlighting)
        {
            int trajectoryID = FindIdOfValue(value);
            // An ID was found
            if (trajectoryID != 0)
            {
                HighlightTrajectory(trajectoryID);
                HighlightBar(trajectoryID);
            }
        }
    }

    // Connected to UI toggle component to set whether to do highlighting
    public void HighlightingToggle(bool isOn)
    {
        doHighlighting = isOn;
        if (isOn == true)
        {
            slider.enabled = true;
            HighlightSliderChanged(slider.value);
        } else
        {
            slider.enabled = false;
            ResetTrajectoriesToOriginal();
            ResetBarsToOriginal();

            // Clear slider value label text
            if (slider)
                slider.GetComponentInChildren<Text>().text = "";
        }
    }

    // Reset material of trajectories' data points and line renderer to their original material
    protected void ResetTrajectoriesToOriginal()
    {
        for (int i = 0; i < trajectoryDataSources.Count; i++)
        {

            DataPoint[] dataPoints = trajectoryDataSources[i].GetComponentsInChildren<DataPoint>();
            foreach (DataPoint dp in dataPoints)
            {
                dp.ResetMaterial();
            }

            // Reset line renderer material to original material based on a data points original material
            LineRenderer lr = trajectoryDataSources[i].GetComponentInChildren<LineRenderer>();
            lr.material = dataPoints[0].GetOriginalMaterial();
        }
    }

    // Reset bars of bar chart to their original material
    protected void ResetBarsToOriginal()
    {
        foreach (BarProperty bar in barProperties)
        {
            bar.ResetBarMatToOriginal();
            bar.LabelContainer.SetActive(true);
        }
    }

    private float[] GetColFromInputSource(int index)
    {
        return inputVarDataSource[0].GetCol(inputVarDataSource[0].dataArray, index);
    }

    public bool GetDoHighlighting()
    {
        return doHighlighting;
    }
    public Material GetHighlightMat()
    {
        return highlightingMaterial;
    }
}
