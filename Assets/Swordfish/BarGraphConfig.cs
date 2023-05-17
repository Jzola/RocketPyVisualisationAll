using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using System.IO;
using System.Linq;

public class BarGraphConfig : MonoBehaviour
{
    [Header("Inputs")]
    public string inputFolderName = "Default_Inputs";
    protected string inputFolderPath = "/Resources/AdditionalOutputs/";
    public List<string> availableInputFolders;
    public List<string> avaliableInputVariables;

    private GameObject graph;
    // Start is called before the first frame update
    void Start()
    {
        graph = transform.parent.gameObject;

        //find the folder names in the Additional Outputs Folder
        DirectoryInfo[] directories = new DirectoryInfo(Application.dataPath + inputFolderPath).GetDirectories();

        //Get the different types of datasets where the input variable was changed (burnout, temperature).
        availableInputFolders = new List<string>();
        for (int i = 0; i < directories.Length; i++)
        {
            availableInputFolders.Add(directories[i].Name);
        }

        // Get the available input variables from the all_inputs.csv file
        avaliableInputVariables.AddRange(File.ReadLines(Application.dataPath + inputFolderPath + inputFolderName + "/inputData/all_inputs.csv").First().Split(','));
    }

    public void regenerateInputVars()
    {
        // Get the available input variables from the all_inputs.csv file
        avaliableInputVariables.AddRange(File.ReadLines(Application.dataPath + inputFolderPath + inputFolderName + "/inputData/all_inputs.csv").First().Split(','));
    }

    [ContextMenu("DeleteGraph")] 
    public void deleteGraph()
    {
        Destroy(transform.parent.gameObject);
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
