using IATK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IATK.DataSource;

public class GraphConfig : GraphAxes
{
    public enum GraphType {SCATTER, BAR};
    private GameObject graph;

    // Animates the points on update if true, teleports if false.
    public bool tweenPointsOnUpdate = true;
    public bool waitForAllPointsToMove = false;
    private bool variablesInitialised = false;

    [Range(-1, 29)] // Can't adjust at runtime, so using the currently known max
    public int selectedTrajectory = -1; // -1 = All, else an individual trajectory
    private int previousSelection = -1;
    private Filtering filter;

    // Focus data that changes between different inputs
    private CSVDataSource focusData = null;
    public string focusID = "None";
    public string focusEngine = "None";
    public string focusType = "None";
    public string focusValue = "None";


    // Start is called before the first frame update
    void Start()
    {
        graph = transform.parent.gameObject;
        filter = graph.GetComponentInChildren<Filtering>();
    }

    // Called every frame
    void Update()
    {
        // If the variables haven't been initialised, attenmpt to do so
        if (!variablesInitialised)
        {
            // Sets the axis variables of the graph, once the graph is made
            Visualisation visualisation = graph.GetComponentInChildren<Visualisation>();
            if (visualisation != null)
            {
                setGraphAxisVariables(graph);
                variablesInitialised = true;
            }
        } 
        else if (previousSelection != selectedTrajectory)
        {
            selectTrajectory(selectedTrajectory);
            previousSelection = selectedTrajectory;
        }
    }

    // Filters out all trajectories except given index, if -1 show all
    public void selectTrajectory(int index)
    {
        filter.ResetFilters();
        if (index >= 0)
        {
            // Sets focus variables
            focusData = filter.FilterSingle(index);
            focusID = focusData.data.name;
            CSVDataSource inputData = focusData.GetComponentInParent<DataFiles>().input;
            // Finds the engine used for the current trajectory
            focusEngine = inputData.getOriginalString(inputData.findCol("motor type"), inputData["motor type"].Data[index] * (inputData["motor type"].MetaData.categoryCount - 1));
            
             if (focusType != "None")
            {
                try
                {
                    DimensionData data = inputData[focusType];
                    focusValue = (data.MetaData.minValue + (data.Data[index] * (data.MetaData.maxValue - data.MetaData.minValue))).ToString(); // Un-normalise the data
                    // TODO add more checking for proper variables and multi-focus strings
                }
                catch (System.Exception e)
                {
                    Debug.Log(e);
                }
            }            

            RocketAnimationUI rocketUI = graph.GetComponentInChildren<RocketAnimationUI>();
            rocketUI.getDropdown().SetValueWithoutNotify(index);
            rocketUI.updateSelectedTrajectory();

        } 
        else
        {
            // Clears focus variables
            focusData = null;
            focusID = "None";
            focusEngine = "None";
            focusValue = "None";
        }

    }

    // Deletes the entire instantiated graph prefab from the scene
    [ContextMenu("Delete Graph")]
    public void DeleteGraph()
    {
        Destroy(graph);
    }

    // Update the graph based on the given settings
    [ContextMenu("Update Graph")]
    public void UpdateGraph()
    {
        setGraphDimensions(dimensions);

        // Set the axis variables and update trajectories
        setGraphAxisVariables(graph);
        StartCoroutine(updateTrajectories(graph));

        // Resets rocket animation data
        RocketAnimation rocket = graph.GetComponentInChildren<RocketAnimation>();
        rocket.setSelectedTrajectory(0);
    }

    private IEnumerator updateTrajectories(GameObject graph)
    {
        DataFiles dataFiles = graph.GetComponentInChildren<DataFiles>();

        // Finds and updates all the previous points with the new axes
        foreach (Transform dataSet in dataFiles.gameObject.transform)
        {
            VisualisationPoints trajectory = dataSet.GetComponentInChildren<VisualisationPoints>();
            if (trajectory != null)
            {
                trajectory.tweenPointsOnUpdate = tweenPointsOnUpdate;
                dataFiles.UpdateTrajectory(trajectory);

                // Prevent points from moving during coroutine
                if (waitForAllPointsToMove) trajectory.pointsNeedUpdating = false;
            }
            yield return null;
        }

        // Enables the points to move once coroutines are over
        if (waitForAllPointsToMove)
        {
            foreach (Transform dataSet in dataFiles.gameObject.transform)
            {
                VisualisationPoints trajectory = dataSet.GetComponentInChildren<VisualisationPoints>();
                if (trajectory != null)
                {
                    trajectory.pointsNeedUpdating = trajectory.tweenPointsOnUpdate;
                }
            }
        }
    }

    // Changes the type of the graph
    public void setGraphType(GraphType type)
    {
        // TODO: implement method
        switch (type)
        {
            case GraphType.SCATTER:
                break;
            case GraphType.BAR:
                break;
            default:
                break;
        }
    }

    // Abstracts the given variable on the graph, default is time
    public void setAbstractedVariable(string var)
    {
        // TODO: Change the graphs abstracted variable from time to var
    }

    // Changes the input dataset
    public void setInputDataset()
    {
        // TODO: Change the dataset from the default to a given dataset
    }

    // Changes the input value of the given dataset
    public void setInputDatasetValue()
    {
        // TODO: Change the value of the given dataset (e.g. length of rocket, weather condition ...)
    }
}
