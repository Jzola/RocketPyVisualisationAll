using IATK;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class GraphMenuSetCreator : MonoBehaviour
{
    public GameObject GraphPrefab;
    public TextAsset variableExtractionFile;

    public string xAxis = " Y (m)";
    public string yAxis = " Z (m)";
    public string zAxis = " X (m)";


    
    [Header("Available Variables:")]
    [Tooltip("List of variables found in the first default csv output file")]
    public List<string> variables;

    private List<GameObject> createdGraphs;


    // Start is called before the first frame update
    void Start()
    {
        createdGraphs = new List<GameObject>();

        // Creates a list of variables from the given file
        variables = new List<string>();
        variables.AddRange(variableExtractionFile.text.Substring(0, variableExtractionFile.text.IndexOf(System.Environment.NewLine)).Split(','));
    }

    [ContextMenu("Create Graph")]
    // Creates a graph, based on the given prefab and set axes
    public void CreateGraph()
    {
        // Creates and adds graph instance to list
        GameObject graph = Instantiate(GraphPrefab);
        createdGraphs.Add(graph);

        // Sets the axes for the graph instance
        Visualisation visualisation = graph.GetComponentInChildren<Visualisation>();
        visualisation.xDimension = new DimensionFilter { Attribute = xAxis };
        visualisation.yDimension = new DimensionFilter { Attribute = yAxis };
        visualisation.zDimension = new DimensionFilter { Attribute = zAxis };
    }

    [ContextMenu("Delete Last Graph")]
    public void DeleteLastGraph()
    {
        if (createdGraphs.Count < 1) return;

        GameObject graph = createdGraphs[createdGraphs.Count - 1];
        createdGraphs.Remove(graph);
        Destroy(graph);
    }

/*    [ContextMenu("Update Last Graph")]
    public void UpdateLastGraph()
    {
        if (createdGraphs.Count < 1) return;

        GameObject graph = createdGraphs[createdGraphs.Count - 1];

        // Sets the axes for the graph instance
        Visualisation visualisation = graph.GetComponentInChildren<Visualisation>();
        visualisation.xDimension = new DimensionFilter { Attribute = xAxis };
        visualisation.yDimension = new DimensionFilter { Attribute = yAxis };
        visualisation.zDimension = new DimensionFilter { Attribute = zAxis };

        DataFiles dataFiles = graph.GetComponentInChildren<DataFiles>();

        foreach (Transform child in dataFiles.gameObject.transform)
        {
            Destroy(child.GetComponentInChildren<CSVDataSource>().obj);
        }

        dataFiles.addNewPoints();
    }
*/
}
