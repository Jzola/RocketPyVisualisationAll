using IATK;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraphCommon : MonoBehaviour
{
    public TextAsset variableExtractionFile;

    // Axes that make up the graph.
    [Header("Axes")]
    public string xAxis = " Y (m)";
    public string yAxis = " Z (m)";
    public string zAxis = " X (m)";
    public int dimensions = 3;

    private const string unusedAxisID = "Undefined";

    [Header("Available Variables for Axes:")]
    [Tooltip("List of variables found in the first default csv output file")]
    public List<string> variables;

    [Header("Inputs")]
    public string inputFolderName = "Default_Inputs";
    protected string inputFolderPath = "/Resources/AdditionalOutputs/";
    public List<string> availableInputs;

    // Start is called before the first frame update
    protected void Initialise()
    {
        // Creates a list of variables from the given file
        variables = new List<string>();
        variables.AddRange(variableExtractionFile.text.Substring(0, variableExtractionFile.text.IndexOf(System.Environment.NewLine)).Split(','));

        //find the folder names in the Additional Outputs Folder
        DirectoryInfo[] directories = new DirectoryInfo(Application.dataPath + inputFolderPath).GetDirectories();

        //Get the different types of datasets where the input variable was changed (burnout, temperature).
        availableInputs = new List<string>();
        for (int i = 0; i < directories.Length; i++)
        {
            availableInputs.Add(directories[i].Name);
        }
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

        // Sets dimensions based on chosen number and if the axes are valid
        visualisation.xDimension = variables.Contains(xAxis) ? xAxis : unusedAxisID;
        visualisation.yDimension = dimensions >= 2 && variables.Contains(yAxis) ?  yAxis  : unusedAxisID;
        visualisation.zDimension = dimensions >= 3 && variables.Contains(zAxis) ?  zAxis  : unusedAxisID;
    }

    // Get the focus variable for which input changes between simulations, acquired from the variablefocusmapping file
    public string getFocusType()
    {
        string focusType = "None";
        
        // Reads and checks map for an input variable corresponding to the input folder, if there is one
        string[] varMap = File.ReadAllLines(Application.dataPath + inputFolderPath + "VariableFocusMapping.txt");
        foreach (string var in varMap)
        {
            string[] pair = var.Split(':');
            if (pair[0].Equals(inputFolderName))
            {
                focusType = pair[1];
            }
        }
        return focusType;
    }
}
