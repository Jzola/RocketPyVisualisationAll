using IATK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphConfig : GraphAxes
{
    public enum GraphType {SCATTER, BAR};

    // Animates the points on update if true, teleports if false.
    public bool tweenPointsOnUpdate = true;
    private bool variablesInitialised = false;
    public int dimensions = 3;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Called every frame
    void Update()
    {
        // If the variables haven't been initialised, attenmpt to do so
        if (!variablesInitialised)
        {
            // Sets the axis variables of the graph, once the graph is made
            Visualisation visualisation = transform.parent.GetComponentInChildren<Visualisation>();
            if ( visualisation != null)
            {
                xAxis = visualisation.xDimension.Attribute;
                yAxis = dimensions >= 2 ? visualisation.yDimension.Attribute : "Undefined";
                zAxis = visualisation.zDimension.Attribute;
                variablesInitialised = true;
            }
        }
    }

    // Deletes the entire instantiated graph prefab from the scene
    [ContextMenu("Delete Graph")]
    public void DeleteGraph()
    {
        Destroy(transform.parent.gameObject);
    }

    // Update the graph based on the given settings
    [ContextMenu("Update Graph")]
    public void UpdateGraph()
    {
        setGraphDimensions(dimensions);
        string unusedAxis = "Undefined";

        // Get the whole graph object
        GameObject graph = transform.parent.gameObject;

        // Sets the axes for the graph instance
        Visualisation visualisation = graph.GetComponentInChildren<Visualisation>();
        visualisation.xDimension = new DimensionFilter { Attribute = xAxis };
        visualisation.yDimension = dimensions >= 2 ? new DimensionFilter { Attribute = yAxis } : unusedAxis;
        visualisation.zDimension = dimensions >= 3 ? new DimensionFilter { Attribute = zAxis } : unusedAxis;

        DataFiles dataFiles = graph.GetComponentInChildren<DataFiles>();

        // Finds and updates all the previous points with the new axes
        foreach (Transform dataSet in dataFiles.gameObject.transform)
        {
            VisualisationPoints trajectory = dataSet.GetComponentInChildren<VisualisationPoints>();
            if (trajectory != null)
            {
                trajectory.tweenPointsOnUpdate = tweenPointsOnUpdate;
                dataFiles.UpdateTrajectory(trajectory);
            }
        }

        // Resets rocket animation data
        RocketAnimation rocket = graph.GetComponentInChildren<RocketAnimation>();
        rocket.setSelectedTrajectory(0);
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

    // Changes the number of dimensions used on the graph; 1, 2 or 3. Does not update the graph, call UpdateGraph() to apply changes
    // Number will be rounded to the closest valid dimension if too large or small
    public void setGraphDimensions(int dimensions)
    {
        this.dimensions = Mathf.Clamp(dimensions, 1, 3);
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
