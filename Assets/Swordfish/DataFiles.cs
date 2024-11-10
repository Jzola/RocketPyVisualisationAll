using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;

public class DataFiles : MonoBehaviour
{
    [SerializeField]
    protected GameObject visualisationPrefab;

    // Dimension axis information
    public float[] dimensionMin { get; set; }
    public float[] dimensionMax { get; set; }
    private int maxIndexX = 0;
    private int maxIndexY = 0;
    private int maxIndexZ = 0;

    // Simulation files
    [SerializeField]
    protected string scenario = "Scenario1";
    [SerializeField]
    private string rocketId;
    private string path = "/Resources/Scenarios/";
    protected List<CSVDataSource> files;
    [System.NonSerialized]
    public CSVDataSource input;

    private float colourCounter = 0;
    private float sizeCounter = 0;

    public void SetVisualisationPrefab(GameObject prefab)
    {
        visualisationPrefab = prefab;
    }

    public void SetRocketId(string id)
    {
        rocketId = id;
    }

    public void setScenario(string scenario)
    {
        this.scenario = scenario;
    }

    public string getSimulationPath()
    {
        return path + scenario;
    }

    public void setSimulationFilesCoroutine()
    {
        StartCoroutine(setSimulationFiles());
    }

    public void initialiseDataSet()
    {
        // First find files and create csvDataSource objects
        files = new List<CSVDataSource>();
        CreateCSVDataSource();

        dimensionMin = new float[files[0].DimensionCount];
        dimensionMax = new float[files[0].DimensionCount];

    }

    public IEnumerator setSimulationFiles()
    {
        // For each data file, create the trajectory within the visualisation object.
        for (int i = 0; i < files.Count; i++)
        {
            // Rescale the values based upon global min/max
            files[i].repopulate(dimensionMin, dimensionMax);

            CreateTrajectory(i);

            yield return null;
        }

    }


    // For each csv file in the directory, create a csvDataSourceObject
    private void CreateCSVDataSource()
    {
        // Makes sure to sort the files properly
        string[] filePaths = Directory.GetFiles(Application.dataPath + (path + scenario + '/' + rocketId), "*.csv").OrderBy(f => Regex.Replace(f, "[0-9]+", match => match.Value.PadLeft(5, '0'))).ToArray();
        string[] inputFile = Directory.GetFiles(Application.dataPath + (path + scenario + '/' + rocketId + "/inputData"), "*.csv");

        GameObject inputDataObj = new GameObject("InputData");
        inputDataObj.transform.SetParent(this.transform, false);
        inputDataObj.AddComponent<CSVDataSource>();
        TextAsset inputDataAsset = TextfromFile(inputFile[0]);
        inputDataAsset.name = "InputData";

        // Load the simulation data into the csvDataSource object
        inputDataObj.GetComponent<CSVDataSource>().data = inputDataAsset;
        inputDataObj.GetComponent<CSVDataSource>().loadHeader();
        inputDataObj.GetComponent<CSVDataSource>().load();
        input = inputDataObj.GetComponent<CSVDataSource>();

        for (int i = 0; i < filePaths.Length; i++)
        {
            // Create new game object with CSVDataSource component
            GameObject dataSourceObj = new GameObject("DataSource" + (i + 1));
            dataSourceObj.transform.SetParent(this.transform, false);

            dataSourceObj.AddComponent<CSVDataSource>();

            // Set CSVDataSource data to file data.
            TextAsset data = TextfromFile(filePaths[i]);

            // Get the file name of the file and set as data name
            // For identification in ChartLinkingManager
            data.name = filePaths[i].Substring(filePaths[i].LastIndexOf("\\") + 1, filePaths[i].LastIndexOf(".") - filePaths[i].LastIndexOf("\\") - 1);

            // For Apple users 
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                data.name = filePaths[i].Substring(filePaths[i].LastIndexOf("/") + 1, filePaths[i].LastIndexOf(".") - filePaths[i].LastIndexOf("/") - 1);
            }

            // Load the simulation data into the csvDataSource object
            dataSourceObj.GetComponent<CSVDataSource>().data = data;
            dataSourceObj.GetComponent<CSVDataSource>().loadHeader();
            dataSourceObj.GetComponent<CSVDataSource>().load();
            files.Add(dataSourceObj.GetComponent<CSVDataSource>());
        }
    }

    // Determine the min and max values of each variable from csv file(s)
    public void GetMinMax()
    {
        // TODO
        // INEFFICIENT?? TEMP FIX 
        // Determine global min/max for scaling
        // Loop each file
        for (int i = 0; i < files.Count; i++)
        {
            // Loop each variable in files
            for (int j = 0; j < files[i].DimensionCount; j++)
            {
                // Get min and max values for variable and compare against current
                // global min/max
                if (i == 0)
                {
                    dimensionMin[j] = files[i].getDimensions()[j].MetaData.minValue;
                    dimensionMax[j] = files[i].getDimensions()[j].MetaData.maxValue;
                }
                else if (files[i].getDimensions()[j].MetaData.minValue < dimensionMin[j])
                {
                    dimensionMin[j] = files[i].getDimensions()[j].MetaData.minValue;
                }

                if (files[i].getDimensions()[j].MetaData.maxValue > dimensionMax[j])
                {
                    dimensionMax[j] = files[i].getDimensions()[j].MetaData.maxValue;

                    // TODO VERY inelegant solution
                    // Finding the file indexes which have the largest x,y,z values so we
                    // can set the axis ticks correctly
                    if (j == 1)
                    {
                        maxIndexX = i;
                    }
                    else if (j == 2)
                    {
                        maxIndexY = i;
                    }
                    else if (j == 3)
                    {
                        maxIndexZ = i;
                    }
                }
            }
        }
    }

    // Creates trajectory data objects 
    protected virtual void CreateTrajectory(int fileIndex)
    {
        Color colour = Color.white;
        // Create the Visualisation object for respective trajectory.
        Visualisation visualisation = Instantiate(visualisationPrefab, transform).GetComponent<Visualisation>();
        visualisation.dataSource = files[fileIndex];
        //Change visualisation properties based on the scenario
        switch (scenario)
        {
            //Colour coding for scenario 1. First trajectory is white and becomes progressively more red
            case "Scenario1":
                colour = new Color(1, 1 - colourCounter, 1 - colourCounter, 1);
                visualisation.colour = colour;
                colourCounter += .033f;
                break;
            // Colour and size coding for scenario 2. Change the colour to represent changing wind speed every 10 trajectories
            // Within each wind speed increase the size of the point to represent increasing variable size
            case "Scenario2":
                if (fileIndex < 10)
                {
                    colour = Color.blue;
                    visualisation.colour = colour;
                }
                else if (fileIndex < 20)
                {
                    colour = Color.green;
                    visualisation.colour = colour;
                }
                else if (fileIndex < 30)
                {
                    colour = new Color(1, 0.5f, 0f, 1);
                    visualisation.colour = colour;
                }
                else if (fileIndex < 40)
                {

                    visualisation.colour = Color.red;
                }
                visualisation.size = 0.3f + sizeCounter;
                sizeCounter += .05f;
                // Reset point size every 10 trajectories
                if (fileIndex > 0 && (fileIndex + 1) % 10 == 0)
                    sizeCounter = 0;
                break;
            // Default to white trajectory colour in other cases i.e. scenario 3
            default:
                colour = Color.white;
                visualisation.colour = colour;
                break;
        }
        visualisation.CreateVisualisation(AbstractVisualisation.VisualisationTypes.SCATTERPLOT);
        BigMesh mesh = visualisation.theVisualizationObject.viewList[0].BigMesh;

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = colour;

        // Create the VisualisationLine object for this trajectory
        GameObject line = new GameObject();
        line.SetActive(false);
        line.AddComponent<VisualisationLine>();
        line.GetComponent<VisualisationLine>().setVisualisationMesh(mesh);
        line.GetComponent<VisualisationLine>().setLineMaterial(mat);
        line.transform.SetParent(visualisation.transform, false);

        line.SetActive(true);
    }

    // Creates TextAsset object from a filepath
    private TextAsset TextfromFile(string path)
    {
        var sr = new StreamReader(path);
        string contents = sr.ReadToEnd();
        sr.Close();
        return new TextAsset(contents);
    }

    // Returns list of data objects
    public List<CSVDataSource> GetFiles()
    {
        return files;
    }

    // Returns CSVDataSource based on the file name it is created from
    public CSVDataSource GetFile(string fileName)
    {
        CSVDataSource file = null;
        foreach (CSVDataSource source in files)
        {
            if (source.data.name == fileName)
            {
                file = source;
                break;
            }
        }
        return file;
    }

    // Adds new points to the existing graph
    public void addNewPoints()
    {
        // For each data file, create the trajectory within the visualisation object.
        for (int i = 0; i < files.Count; i++)
        {
            // Rescale the values based upon global min/max
            files[i].repopulate(dimensionMin, dimensionMax);

            // Create the trajectory data objects
            CreateTrajectory(i);
        }
    }

    public void scaleData(float[] min, float[] max)
    {
        foreach (CSVDataSource file in files)
            file.repopulate(min, max);
    }

    // Deletes all children (trajectories, points, etc) of this object.
    public void DestroyTrajectories()
    {
        colourCounter = 0;
        foreach (Transform child in transform)
        {
            if (child.tag == "Visualisation")
                Destroy(child.gameObject);
        }
    }
}
