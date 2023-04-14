using IATK;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class GraphCreator : GraphAxes
{
    public GameObject GraphPrefab;
    public string inputFolderName = "Default_Inputs";
    private string inputFolderPath = "/Resources/AdditionalOutputs/";

    public string[] availableInputs;

    // Start is called before the first frame update
    void Start()
    {
        // Creates a list of variables from the given file
        variables = new List<string>();
        variables.AddRange(variableExtractionFile.text.Substring(0, variableExtractionFile.text.IndexOf(System.Environment.NewLine)).Split(','));

        DirectoryInfo[] directories = new DirectoryInfo(Application.dataPath + inputFolderPath).GetDirectories();

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
        // Creates graph and sets its axis variables
        GameObject graph = Instantiate(GraphPrefab);
        setGraphAxisVariables(graph);

        //StartCoroutine(graph.GetComponentInChildren<DataFiles>().setSimulationFiles(inputFolderPath, inputFolderName));
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
}
