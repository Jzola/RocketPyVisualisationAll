using IATK;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class GraphMenuSetCreator : GraphAxes
{
    public GameObject GraphPrefab;

    // Start is called before the first frame update
    void Start()
    {
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
        
        // Sets the axes for the graph instance
        Visualisation visualisation = graph.GetComponentInChildren<Visualisation>();
        visualisation.xDimension = new DimensionFilter { Attribute = xAxis };
        visualisation.yDimension = new DimensionFilter { Attribute = yAxis };
        visualisation.zDimension = new DimensionFilter { Attribute = zAxis };

        graph.GetComponentInChildren<GraphConfig>().variables = variables;
    }
}
