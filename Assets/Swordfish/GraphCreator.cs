using IATK;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class GraphCreator : GraphAxes
{
    //Graph prefab is based on the original Input data using different rocket types and weights.
    public GameObject GraphPrefab;
    public string inputFolderName = "Default_Inputs";
    private string inputFolderPath = "/Resources/AdditionalOutputs/";

    public string[] availableInputs;
    public int graphsCreated = 0;
    public float radius = 6;

    // Start is called before the first frame update
    void Start()
    {
        
        // Creates a list of variables from the given file
        variables = new List<string>();
        variables.AddRange(variableExtractionFile.text.Substring(0, variableExtractionFile.text.IndexOf(System.Environment.NewLine)).Split(','));

        //find the folder names in the Additional Outputs Folder
        DirectoryInfo[] directories = new DirectoryInfo(Application.dataPath + inputFolderPath).GetDirectories();

        //Get the different types of datasets where the input variable was changed (burnout, temperature).
        availableInputs = new string[directories.Length];
        for (int i = 0; i < directories.Length; i++)
        {
            availableInputs[i] = directories[i].Name;
        }
    }

    [ContextMenu("Create Graph")]
    // Creates a graph, based on the given prefab and set axes
    public void CreateGraph()
    {
        
        transform.RotateAround(transform.position, Vector3.up, graphsCreated*30);
        graphsCreated++;
        // Creates graph and sets its axis variables
        //transform.RotateAround(transform.position, 30);
        GameObject graph = Instantiate(GraphPrefab,SpawnInCircle(transform.position, radius), transform.rotation);
        setGraphAxisVariables(graph);

        //Change the folder here
        graph.GetComponentInChildren<DataFiles>().setSimulationPath(inputFolderPath, inputFolderName);

        // Sets the variables for the graph config. The config will still automatically get these variables after creations,
        // but the inspector window won't update without this happening before being fully instantiated
        GraphConfig graphConfig = graph.GetComponentInChildren<GraphConfig>();
        graphConfig.variables = variables;
        graphConfig.dimensions = dimensions;
        graphConfig.xAxis = xAxis;
        graphConfig.yAxis = yAxis;
        graphConfig.zAxis = zAxis;

    }

    Vector3 SpawnInCircle(Vector3 center, float radius)
    {
        //float ang = Random.value * 360;
        float ang = graphsCreated * 30;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }
}
