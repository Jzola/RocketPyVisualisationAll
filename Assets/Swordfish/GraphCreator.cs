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

    public GameObject VRCamera;
    Vector3 VRCamOriginalPosition; // = VRCamera.transform.position;
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
        //get the original position of the camera 
        VRCamOriginalPosition = VRCamera.transform.position;
        VRCamOriginalPosition.y = 3;
    }

    [ContextMenu("Create Graph")]
    // Creates a graph, based on the given prefab and set axes
    public void CreateGraph()
    {

        GameObject graph = Instantiate(GraphPrefab, SpawnInCircle(VRCamOriginalPosition, radius), VRCamera.transform.rotation); ;
        
        // Creates graph and sets its axis variables. Move up if there are 6 graphs, or they will overlap
        if(graphsCreated == 5)
        {
            VRCamOriginalPosition.y = VRCamOriginalPosition.y + 6;

        }
        else
        {
            //was needed to fix an unidentified bug where new graphs spawning at y = 40 
            //VRCamOriginalPosition.y = 3;
        }
               
        setGraphAxisVariables(graph);
        
        //rotate the graph for better viewing
        //TODO fix issue where rotations go too high.
        graph.transform.RotateAround(graph.transform.position, Vector3.up, graphsCreated * 60);
        graphsCreated++;


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

        // Set the focus variable for which input changes between simulations
        string focusType = "None";
        string[] varMap = File.ReadAllLines(Application.dataPath + inputFolderPath + "VariableFocusMapping.txt");
        foreach (string var in varMap)
        {
            string[] pair = var.Split(':');
            if (pair[0].Equals(inputFolderName)) {
                focusType = pair[1];
            }
        }
        graphConfig.focusType = focusType;

    }

    //function for spawning new graphs in circle around user. Not really working properly yet.
    Vector3 SpawnInCircle(Vector3 center, float radius)
    {
        //An offset is required to push the graph far enough away from the creator so we can see it
        //possible bug due to values able to be negative
        float offset = 1;
        //TODO adjust the angle so that objects arrange themselves in a circle.
        float ang = graphsCreated * 60;
        Vector3 pos;
        pos.x = center.x + (radius * Mathf.Sin(ang * Mathf.Deg2Rad)) ;
        pos.y = center.y + 1;
        pos.z = center.z + (radius * Mathf.Cos(ang * Mathf.Deg2Rad)); 
        return pos;

    }
}
