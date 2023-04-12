using IATK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphAxes : MonoBehaviour
{
    public TextAsset variableExtractionFile;

    // Axes that make up the graph.
    public string xAxis;
    public string yAxis;
    public string zAxis;
    public int dimensions = 3;

    private const string unusedAxisID = "Undefined";

    [Header("Available Variables:")]
    [Tooltip("List of variables found in the first default csv output file")]
    public List<string> variables;

    // Start is called before the first frame update
    void Start()
    {
        // Creates a list of variables from the given file
        variables = new List<string>();
    }

    // Called every frame
    void Update()
    {
       
    }

    // Changes the number of dimensions used on the graph; 1, 2 or 3. Does not update the graph, call UpdateGraph() to apply changes
    // Number will be rounded to the closest valid dimension if too large or small
    public void setGraphDimensions(int dimensions)
    {
        this.dimensions = Mathf.Clamp(dimensions, 1, 3);
    }

    // Changes the axes on the given graph to the axes currently stored.
    // Will not properly update the graph by itself, the graph must be regenerated afterwards
    public void setGraphAxisVariables(GameObject graph)
    {
        // Sets the axes for the graph instance
        Visualisation visualisation = graph.GetComponentInChildren<Visualisation>();
        visualisation.xDimension = new DimensionFilter { Attribute = xAxis };
        visualisation.yDimension = dimensions >= 2 ? new DimensionFilter { Attribute = yAxis } : unusedAxisID;
        visualisation.zDimension = dimensions >= 3 ? new DimensionFilter { Attribute = zAxis } : unusedAxisID;
    }
}
