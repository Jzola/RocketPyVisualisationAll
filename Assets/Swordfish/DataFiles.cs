using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;

public class DataFiles : MonoBehaviour
{
    public Visualisation visualisation;  // IATK visualisation object
    public GameObject dataPointPrefab;   // Prefab for trajectory data points
    [SerializeField]
    private RocketAnimation rocket;
    [SerializeField]
    private GameObject LegendItemPrefab;

    // Dimension axis information
    private float[] dimensionMin;
    private float[] dimensionMax;
    private int maxIndexX = 0;
    private int maxIndexY = 0;
    private int maxIndexZ = 0;

    // Simulation files
    private string folder = "Default_Inputs";
    private string path = "/Resources/AdditionalOutputs/";
    private List<CSVDataSource> files;
    [System.NonSerialized]
    public CSVDataSource input;


    // Colour Coding
    //[SerializeField]
    private string colourCol = "# Time (s)";
    private Color trajStartCol = new Color(1f, 0.165f, 0.165f);
    private Color trajEndCol = new Color(0.008f, 0.361f, 0.122f);

    private Color[] classifications = new Color[]
    {
        new Color(0.396f, 0.45f, 0.745f, 0.9f),
        new Color(0.372f, 0.624f, 0.216f, 0.9f),
        new Color(1f, 1f, 1f, 0.9f),                // white for apogee
        new Color(0.576f, 0.29f, 0.8f, 0.9f),
        new Color(0.847f, 0.302f, 0.2f, 0.9f),
        new Color(0.651f, 0.475f, 0.192f, 0.9f),
        new Color(0.678f, 0.753f, 1f, 0.9f),
        new Color(0.898f, 0.773f, 0.071f, 0.9f)
    };

    private Material[] dataPointMats;

    // TODO make more dynamic
    private int[] flightStageIndexes = new int[] { 18, 19, 20, 21, 22, 23, 24, 25};

    private void Start()
    {
        createMaterials();
        StartCoroutine(setSimulationFiles());
    }

    public void setSimulationPath(string path, string folder)
    {
        // Use old data if new data paths isn't found
        if (Directory.Exists(Application.dataPath + path + folder))
        {
            this.path = path;
            this.folder = folder;
        }
    }

    public IEnumerator setSimulationFiles()
    {
        // First find files and create csvDataSource objects
        files = new List<CSVDataSource>();
        CreateCSVDataSource();

        dimensionMin = new float[files[0].DimensionCount];
        dimensionMax = new float[files[0].DimensionCount];

        GetMinMax();

        // For each data file, create the trajectory within the visualisation object.
        for (int i = 0; i < files.Count; i++)
        {
            // Rescale the values based upon global min/max
            files[i].repopulate(dimensionMin, dimensionMax);

            if (visualisation != null)
            {
                // Create the trajectory data objects
                CreateTrajectory(i);
            }
            yield return null;
        }

        if (visualisation != null)
        {
            // After all trajectories have been created, update axis ticks
            UpdateAxisTicks();

            // After final view has loaded, delete it from the visualisation object as
            // all trajectory data is in visualisationPoints and visualisationLines objects .
            visualisation.destroyView();

            // Add colour coding information to the legend
            UpdateLegend();
        }
    }

    private void createMaterials()
    {
        // Create the material objects
        dataPointMats = new Material[]
        {
            new Material(Shader.Find("Standard")),
            new Material(Shader.Find("Standard")),
            new Material(Shader.Find("Standard")),
            new Material(Shader.Find("Standard")),
            new Material(Shader.Find("Standard")),
            new Material(Shader.Find("Standard")),
            new Material(Shader.Find("Standard")),
            new Material(Shader.Find("Standard")),
        };

        // Set the material colours
        for (int i = 0; i < dataPointMats.Length; i++)
        {
            dataPointMats[i].SetFloat("_Mode", 3);
            dataPointMats[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            dataPointMats[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // dataPointMats[i].SetInt("_ZWrite", 0);
            dataPointMats[i].DisableKeyword("_ALPHATEST_ON");
            dataPointMats[i].DisableKeyword("_ALPHABLEND_ON");
            dataPointMats[i].EnableKeyword("_ALPHAPREMULTIPLY_ON");
            dataPointMats[i].renderQueue = 3000;
            dataPointMats[i].color = classifications[i];
        }
    }

    // For each csv file in the directory, create a csvDataSourceObject
    private void CreateCSVDataSource()
    {
        // Makes sure to sort the files properly
        string[] filePaths = Directory.GetFiles(Application.dataPath + (path + folder), "*.csv").OrderBy(f => Regex.Replace(f, "[0-9]+", match => match.Value.PadLeft(5, '0'))).ToArray();
        string[] inputFile = Directory.GetFiles(Application.dataPath + (path + folder + "/inputData"), "*.csv");

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
    private void GetMinMax()
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

    // Creates trajectory data objects (BigMesh, LineRenderer, MeshCollider, VisualisationPoints)
    private void CreateTrajectory(int fileIndex)
    {
        // Create the BigMesh object for respective trajectory.
        visualisation.dataSource = files[fileIndex];
        visualisation.CreateVisualisation(AbstractVisualisation.VisualisationTypes.SCATTERPLOT);
        BigMesh mesh = visualisation.theVisualizationObject.viewList[0].BigMesh;

        // Create a randomly coloured material to use for the VisualisationLine and VisualisationPoints objects
        var rand = new System.Random();
        Material mat = new Material(Shader.Find("Standard"));
        Color color = new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble(), 1);
        mat.color = color;

        // Create the VisualisationLine object for this trajectory
        GameObject line = new GameObject();
        line.SetActive(false);
        line.AddComponent<VisualisationLine>();
        line.GetComponent<VisualisationLine>().setVisualisationMesh(mesh);
        line.GetComponent<VisualisationLine>().setLineMaterial(mat);
        line.transform.SetParent(files[fileIndex].transform, false);

        line.SetActive(true);

        // Add the LineRenderer to the list in rocketTrajectory
        if (rocket != null)
        {
            rocket.lineList.Add(line.GetComponent<LineRenderer>());
        } 

        // Create the VisualisationPoints object for this trajectory
        GameObject point = new GameObject();
        point.SetActive(false);
        point.AddComponent<VisualisationPoints>();
        point.GetComponent<VisualisationPoints>().setVisualisationMesh(mesh);
        point.GetComponent<VisualisationPoints>().setDataPointPrefab(dataPointPrefab);
        point.GetComponent<VisualisationPoints>().setPointMaterial(mat);
        point.transform.SetParent(files[fileIndex].transform, false);
        point.SetActive(true);

        // Get the flight stage times for the trajectory
        // -1 because index starts at 0, but trajectory ID starts at 1.
        float[] stageTimes = input.GetCols(flightStageIndexes, (files[fileIndex].GetID()-1));

        // Create the data points for trajectory
        point.GetComponent<VisualisationPoints>().CreatePoints(stageTimes, colourCol, dataPointMats);
    }

    // Updates an existing trajectory using its visualisationpoints component
    public void UpdateTrajectory(VisualisationPoints visualisationPoints)
    {
        // Create the BigMesh object for respective trajectory.
        visualisation.dataSource = visualisationPoints.GetComponentInParent<CSVDataSource>();
        visualisation.CreateVisualisation(AbstractVisualisation.VisualisationTypes.SCATTERPLOT);
        BigMesh mesh = visualisation.theVisualizationObject.viewList[0].BigMesh;

        // Set the Visualisation points/line components mesh to the new BigMesh
        visualisationPoints.transform.parent.GetComponentInChildren<VisualisationLine>().setVisualisationMesh(mesh);
        visualisationPoints.setVisualisationMesh(mesh);

        visualisationPoints.updatePoints();

        // Removes the mesh from the ingame scene
        DestroyImmediate(mesh.gameObject);
    }

    // Adds the colour coding information to the visualisation legend
    private void UpdateLegend()
    {
        // Change box colour for first prefab
        Image colourBox = LegendItemPrefab.GetComponentInChildren<Image>();
        colourBox.color = classifications[0];
        Text variableName = LegendItemPrefab.GetComponentInChildren<Text>();
        variableName.text = input.getDimensions()[flightStageIndexes[0]].Identifier;
        float yPos = LegendItemPrefab.GetComponent<RectTransform>().rect.height * 1.5f;
        
        // Get the name of each variable used for colour coding
        for (int i = 1; i < flightStageIndexes.Length; i++)
        {
            // For calculating the height offset 
            int multiplier = i;

            // Create the new legend item object
            GameObject newItem = Instantiate(LegendItemPrefab);
            newItem.transform.parent = LegendItemPrefab.transform.parent;
            newItem.transform.localScale = LegendItemPrefab.transform.localScale;
            newItem.transform.position = LegendItemPrefab.transform.position;
            newItem.transform.rotation = LegendItemPrefab.transform.rotation;
            Vector2 newPos = new Vector2(newItem.GetComponent<RectTransform>().anchoredPosition.x, newItem.GetComponent<RectTransform>().anchoredPosition.y - (yPos*multiplier));
            newItem.GetComponent<RectTransform>().anchoredPosition = newPos;

            // Change box colour
            colourBox = newItem.GetComponentInChildren<Image>();
            variableName = newItem.GetComponentInChildren<Text>();
            colourBox.color = classifications[i];
            variableName.text = input.getDimensions()[flightStageIndexes[i]].Identifier;
        } 

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

    // Update the axis ticks 
    private void UpdateAxisTicks()
    {
        // Update X axis
        DestroyImmediate(visualisation.theVisualizationObject.X_AXIS);
        visualisation.dataSource = files[maxIndexY];
        visualisation.theVisualizationObject.ReplaceAxis(AbstractVisualisation.PropertyType.X);

        // Update Y axis
        DestroyImmediate(visualisation.theVisualizationObject.Y_AXIS);
        visualisation.dataSource = files[maxIndexZ];
        visualisation.theVisualizationObject.ReplaceAxis(AbstractVisualisation.PropertyType.Y);

        // Update Z axis
        DestroyImmediate(visualisation.theVisualizationObject.Z_AXIS);
        visualisation.dataSource = files[maxIndexX];
        visualisation.theVisualizationObject.ReplaceAxis(AbstractVisualisation.PropertyType.Z);
    }

    // Sets the visualisation key text to the launch site latitude and longitude
    public void SetKey(float lat, float lon)
    {
        if (visualisation != null)
        {
            string label = "Latitude: " + lat + "\nLongitude: " + lon;
            visualisation.SetKey(label, 0.4f);
        }
    }

        public void SetKey(string label)
    {
        visualisation.SetKey(label, 0.4f);
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

        // After all trajectories have been created, update axis ticks
        UpdateAxisTicks();

        // After final view has loaded, delete it from the visualisation object as
        // all trajectory data is in visualisationPoints and visualisationLines objects .
        visualisation.destroyView();

        // Add colour coding information to the legend
        UpdateLegend();
    }
}
