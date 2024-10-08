using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IATK;
using System.IO;

public class VisualisationManager : MonoBehaviour
{
    private List<AltitudeCheck> altitudeChecks;
    private List<DataFiles> files;
    private List<Visualisation> visualisations;
    private List<string> scenarios;
    private string currentScenario = "";
    private int scenarioCounter = 0;
    private Dictionary<string, List<GameObject>> scenarioObjects;
    private Dictionary<string, float[]> scenarioMin;
    private Dictionary<string, float[]> scenarioMax;
    private List<GameObject> dataSources;
    private bool input = true;

    [SerializeField]
    private GameObject visualisationPrefab;
    [SerializeField]
    private SidePanel sidePanel;
    [SerializeField]
    private QuestionManager questionPanel;
    [SerializeField]
    private GameObject cameraRoot;

    private const string minMaxPath = "/Resources/Scenarios/MinMax/";
    private const int scenario1Length = 5;
    private const int scenario2Length = 5;
    private const int scenario3Length = 10;

    // Start is called before the first frame update
    void Start()
    {
         
        files = new List<DataFiles>(GetComponentsInChildren<DataFiles>());
        visualisations = new List<Visualisation>(GetComponentsInChildren<Visualisation>());

        foreach (Visualisation visualisation in visualisations)
        {
            visualisation.xDimension = " Y (m)";
            visualisation.yDimension = " Z (m)";
            visualisation.zDimension = " X (m)";
        }

        altitudeChecks = new List<AltitudeCheck>(GetComponentsInChildren<AltitudeCheck>(true));
        scenarios = new List<string>();
        scenarioObjects = new Dictionary<string, List<GameObject>>();
        scenarioMin = new Dictionary<string, float[]>();
        scenarioMax = new Dictionary<string, float[]>();
        dataSources = new List<GameObject>();

        //TODO: Load scenario names from directory
        //for now use hard coded names
        scenarios.Add("Scenario1");
        scenarios.Add("Scenario2");
        scenarios.Add("Scenario3");
        //scenarios.Add("Scenario4");

        foreach (string scenario in scenarios)
        {
            initialiseData(scenario);
        }

        loadMinMaxFiles();
        initialiseAltitudePosition();

        //Default to first scenario
        //SetActiveScenario(0);
    }

    public void NextScenario()
    {
        scenarioCounter++;
        SetActiveScenario(scenarioCounter);
        input = false;
        StartCoroutine(waitForLoad());
    }

    public void PrevScenario()
    {
        scenarioCounter--;
        if (scenarioCounter < 0)
        {
            scenarioCounter = scenarios.Count - 1;
        }
        SetActiveScenario(scenarioCounter);
        input = false;
        StartCoroutine(waitForLoad());
    }

    private IEnumerator waitForLoad()
    {
        yield return new WaitForSeconds(2);
        input = true;
    }

    public void RemakeVisualisations()
    {
        foreach (DataFiles file in files)
        {
            //Remove all the old trajectories so new ones can be created
            file.DestroyTrajectories();
            //Update the scenario to load the appropriate data
            file.setScenario(currentScenario);
        }
            
        //initialiseData();

        //StartCoroutine(createVisualisations());
        setDesiredAltitude();
    }

    private void initialiseData(string scenario)
    {   
        List<GameObject> scenarioList = new List<GameObject>();

        for (int i = 0; i < visualisations.Count; i++)
        {
            GameObject scenarioObj = new GameObject(scenario);
            scenarioObj.transform.SetParent(visualisations[i].transform, false);
            scenarioList.Add(scenarioObj);

            GameObject dataFilesObj = new GameObject("DataFiles");
            dataFilesObj.transform.SetParent(scenarioObj.transform, false);

            DataFiles file = dataFilesObj.AddComponent<DataFiles>();
            file.setScenario(scenario);
            file.SetVisualisationPrefab(visualisationPrefab);
            file.SetRocketId("RocketID_" + (i + 1));

            file.initialiseDataSet();
            //Dynamically find the min and max of each dimension across all datasets
            //Needed to normalise all the data to the same scale for comparison
            file.GetMinMax();
            if (!scenarioMin.ContainsKey(scenario))
            {
                scenarioMin.Add(scenario, file.dimensionMin);
            }
            else 
            {
                for (int j = 0; j < file.dimensionMin.Length; j++)
                    if (file.dimensionMin[j] < scenarioMin[scenario][j])
                        scenarioMin[scenario][j] = file.dimensionMin[j];
            }
            if (!scenarioMax.ContainsKey(scenario))
            {
                scenarioMax.Add(scenario, file.dimensionMax);
            }
            else
            {
                for (int j = 0; j < file.dimensionMax.Length; j++)
                    if (file.dimensionMax[j] > scenarioMax[scenario][j])
                        scenarioMax[scenario][j] = file.dimensionMax[j];
            }   
        }

        scenarioObjects.Add(scenario, scenarioList);
    }

    private void setDesiredAltitude()
    {
        foreach (AltitudeCheck altitudeCheck in altitudeChecks)
        {
            altitudeCheck.setHeight(scenarioMax[currentScenario][3], scenarioMin[currentScenario][3]);
        }
    }

    private void initialiseAltitudePosition()
    {
        foreach (AltitudeCheck altitudeCheck in altitudeChecks)
        {
            altitudeCheck.Initialise();
        }
    }

    public void SetActiveScenario(int scenarioNo)
    {
        string scenario = scenarios[scenarioNo];
        
        switch(scenarioNo)
        {
            case 1:
                {
                    break;
                }
            case 2:
                {
                    questionPanel.gameObject.SetActive(false);
                    sidePanel.gameObject.SetActive(true);
                    break;
                }
        }

        if (scenarioObjects.ContainsKey(currentScenario))
        {
            foreach (GameObject scenarioObj in scenarioObjects[currentScenario])
            {
                foreach (DataFiles file in scenarioObj.GetComponentsInChildren<DataFiles>())
                {
                    file.DestroyTrajectories();
                }                
            }
        }
        if (scenarioObjects.ContainsKey(scenario))
        {
            foreach (GameObject scenarioObj in scenarioObjects[scenario])
            {
                foreach (DataFiles file in scenarioObj.GetComponentsInChildren<DataFiles>())
                {
                    file.dimensionMin = scenarioMin[scenario];
                    file.dimensionMax = scenarioMax[scenario];
                    string min = string.Join(",", scenarioMin[scenario]);
                    Debug.Log(scenario + " min: " + min);
                    string max = string.Join(",", scenarioMax[scenario]);
                    Debug.Log(scenario + " max:" + max);
                    file.setSimulationFilesCoroutine();                   
                }
            }           
        }

        foreach (Visualisation visualisation in visualisations)
        {
            updateAxisTicks(visualisation, scenarioNo);
        }

        currentScenario = scenario;
        setDesiredAltitude();
    }

    private void updateAxisTicks(Visualisation visualisation, int scenarioNo)
    {
        // Destroys axes directly, since visulation may have changed and lost axes further down.
        Axis[] oldAxes = visualisation.transform.GetComponentsInChildren<Axis>();
        visualisation.CreateVisualisation(AbstractVisualisation.VisualisationTypes.SCATTERPLOT);
        CSVDataSource dataSource = dataSources[scenarioNo].GetComponent<CSVDataSource>();
        visualisation.dataSource = dataSource;

        for (int i = 0; i < oldAxes.Length; i++)
        {
            Destroy(oldAxes[i].gameObject);
        }

        if (!visualisation.xDimension.Attribute.Equals("Undefined"))
        {
            // Update X axis   
            visualisation.theVisualizationObject.ReplaceAxis(AbstractVisualisation.PropertyType.X);
        }

        if (!visualisation.yDimension.Attribute.Equals("Undefined"))
        {
            // Update Y axis
            visualisation.theVisualizationObject.ReplaceAxis(AbstractVisualisation.PropertyType.Y);
        }

        if (!visualisation.zDimension.Attribute.Equals("Undefined"))
        {
            // Update Z axis
            visualisation.theVisualizationObject.ReplaceAxis(AbstractVisualisation.PropertyType.Z);
        }
    }

    private void loadMinMaxFiles()
    {
        string[] filePaths = Directory.GetFiles(Application.dataPath + (minMaxPath), "*.csv");

        for (int i = 0; i < filePaths.Length; i++)
        {
            GameObject dataSourceObj = new GameObject("Scenario" + (i + 1));
            dataSourceObj.transform.SetParent(this.transform, false);
            
            dataSourceObj.AddComponent<CSVDataSource>();
            TextAsset data = TextfromFile(filePaths[i]);

            dataSourceObj.GetComponent<CSVDataSource>().data = data;
            dataSourceObj.GetComponent<CSVDataSource>().loadHeader();
            dataSourceObj.GetComponent<CSVDataSource>().load();

            dataSources.Add(dataSourceObj);
        }
    }

    private TextAsset TextfromFile(string path)
    {
        var sr = new StreamReader(path);
        string contents = sr.ReadToEnd();
        sr.Close();
        return new TextAsset(contents);
    }
}
