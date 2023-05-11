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

    private GraphSpawnHandler graphHandler;

    [Header("Object to Spawn Around")]
    public GameObject VRCamera;
    Vector3 VRCamOriginalPosition; // = VRCamera.transform.position;
    public GameObject spawnCirclePrefab;

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
        graphHandler = gameObject.AddComponent<GraphSpawnHandler>();
        graphHandler.spawnCirclePrefab = spawnCirclePrefab;
        graphHandler.setSpawnCentre(VRCamera.transform.position);

        //force the height to where it is visible to player
        float graphHeight = 1.5f;
        graphHandler.setSpawnHeight(graphHeight);
    }

    // Called every frame
    void Update()
    {

    }

    // Returns bool whether there is a free space in the graph handler for more graphs or not
    public bool hasFreeSpace()
    {
        return graphHandler.hasFreeSpace();
    }

    public bool canCreateNewGraph()
    {
        return graphHandler.hasFreeSpace() && graphHandler.validSpotSelected();
    }

    [ContextMenu("Create Graph")]
    // Creates a graph, based on the given prefab and set axes
    public void CreateGraph()
    {
        if (!graphHandler.hasFreeSpace()) return;

        GameObject graph = null;

        // Depending on the currently set graphtype enum, that specified graph will be set up
        switch (graphType)
        {
            case GraphType.BAR:
                //graph = Instantiate(barGraphPrefab, SpawnInCircle(VRCamOriginalPosition, radius), VRCamera.transform.rotation);
                graph = Instantiate(barGraphPrefab);

                // Change the folder input folder
                graph.GetComponentInChildren<DataFiles>().setSimulationPath(inputFolderPath, inputFolderName);
                LoadInputVariables liv = graph.GetComponentInChildren<LoadInputVariables>();
                liv.folder = "inputData";
                liv.path = inputFolderPath + inputFolderName + "/";
                break;


            case GraphType.SCATTER:
                //graph = Instantiate(scatterGraphPrefab, SpawnInCircle(VRCamOriginalPosition, radius), VRCamera.transform.rotation);
                graph = Instantiate(scatterGraphPrefab);

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

        graphHandler.add(graph);
    }
}
