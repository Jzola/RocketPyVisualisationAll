using IATK;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using BarGraph.VittorCloud;

public class GraphCreator : GraphCommon
{
    public enum GraphType { SCATTER, BAR };

    //Graph prefab is based on the original Input data using different rocket types and weights.
    [Header("Graphs")]
    public GameObject scatterGraphPrefab;
    public GameObject barGraphPrefab;
    public GraphType graphType = GraphType.SCATTER;

    private int graphsCreated = 0;
    private float radius = 5;
    private float maxGraphsInCircle = 6;

    [Header("Object to Spawn Around")]
    public GameObject VRCamera;
    Vector3 VRCamOriginalPosition; // = VRCamera.transform.position;

    // Start is called before the first frame update
    void Start()
    {
        Initialise();

        //get the original position of the camera 
        VRCamOriginalPosition = VRCamera.transform.position;
        //force the height to where it is visible to player
        VRCamOriginalPosition.y = 3;
    }

    [ContextMenu("Create Graph")]
    // Creates a graph, based on the given prefab and set axes
    public void CreateGraph()
    {
        GameObject graph = null;

        // Depending on the currently set graphtype enum, that specified graph will be set up
        switch (graphType)
        {
            case GraphType.BAR:
                graph = Instantiate(barGraphPrefab, SpawnInCircle(VRCamOriginalPosition, radius), VRCamera.transform.rotation);

                // Change the folder input folder
                graph.GetComponentInChildren<DataFiles>().setSimulationPath(inputFolderPath, inputFolderName);
                LoadInputVariables liv = graph.GetComponentInChildren<LoadInputVariables>();
                liv.folder = "inputData";
                liv.path = inputFolderPath + inputFolderName + "/";
                break;


            case GraphType.SCATTER:
                graph = Instantiate(scatterGraphPrefab, SpawnInCircle(VRCamOriginalPosition, radius), VRCamera.transform.rotation);

                // Pretty jank, but "hides" the bar graph stuff attached to the scatter graph, since the scatter can't be generated without it.
                graph.GetComponentInChildren<BarGraphGenerator>().transform.Translate(0, -9999, 0, graph.transform);
                graph.GetComponentInChildrenWithTag<Canvas>("Highlighting").transform.Translate(0,-9999, 0, graph.transform);
                //graph.GetComponentInChildrenWithTag<Canvas>("Highlighting").

                setGraphAxisVariables(graph);

                // Change the folder input folder
                graph.GetComponentInChildren<DataFiles>().setSimulationPath(inputFolderPath, inputFolderName);

                // Sets the variables for the graph config. The config will still automatically get these variables after creations,
                // but the inspector window won't update without this happening before being fully instantiated
                GraphConfig graphConfig = graph.GetComponentInChildren<GraphConfig>();
                graphConfig.variableExtractionFile = variableExtractionFile;
                graphConfig.variables = variables;
                graphConfig.dimensions = dimensions;
                graphConfig.xAxis = xAxis;
                graphConfig.yAxis = yAxis;
                graphConfig.zAxis = zAxis;
                graphConfig.inputFolderName = inputFolderName;
                graphConfig.availableInputs = availableInputs;
                graphConfig.focusType = graphConfig.getFocusType();
                break;


            default:
                break;
        }

        //rotate the graph for better viewing
        //TODO fix issue where rotations go too high.
        graph.transform.RotateAround(graph.transform.position, Vector3.up, graphsCreated * (360 / maxGraphsInCircle));
        graphsCreated++;

        //  Move up if there are 6 graphs, or they will overlap
        if (graphsCreated == maxGraphsInCircle)
        {
            VRCamOriginalPosition.y = VRCamOriginalPosition.y + 6;

        }
        else
        {
            //was needed to fix an unidentified bug where new graphs spawning at y = 40 
            //VRCamOriginalPosition.y = 3;
        }
    }

    //function for spawning new graphs in circle around user. Not really working properly yet.
    Vector3 SpawnInCircle(Vector3 center, float radius)
    {
        //An offset is required to push the graph far enough away from the creator so we can see it
        //possible bug due to values able to be negative
        float offset = 1;
        //TODO adjust the angle so that objects arrange themselves in a circle.
        float ang = graphsCreated * (360 / maxGraphsInCircle);
        Vector3 pos;
        pos.x = center.x + (radius * Mathf.Sin(ang * Mathf.Deg2Rad)) ;
        pos.y = center.y + 1;
        pos.z = center.z + (radius * Mathf.Cos(ang * Mathf.Deg2Rad)); 
        return pos;

    }
}
