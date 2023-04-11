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
        // If the variables haven't been added yet, check the variable file and try to add them
        //if (variables.Count == 0 && variableExtractionFile != null)
        //{
        //    variables.AddRange(variableExtractionFile.text.Substring(0, variableExtractionFile.text.IndexOf(System.Environment.NewLine)).Split(','));
        //}
    }
}
