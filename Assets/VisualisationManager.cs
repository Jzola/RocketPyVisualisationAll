using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using System;

public class VisualisationManager : MonoBehaviour
{
    private List<AltitudeCheck> altitudeChecks;
    private List<DataFiles> files;
    private List<ScenarioSwitcher> visualisations;
    private List<string> scenarios;
    private string currentScenario = "";
    private Dictionary<string, List<GameObject>> scenarioObjects;

    private float[] globalMin;
    private float[] globalMax;

    public GameObject visualisationPrefab;


    // Start is called before the first frame update
    void Start()
    {
        foreach (Visualisation visualisation in GetComponentsInChildren<Visualisation>())
        {
            visualisation.xDimension = " Y (m)";
            visualisation.yDimension = " Z (m)";
            visualisation.zDimension = " X (m)";
        }    
        files = new List<DataFiles>(GetComponentsInChildren<DataFiles>());
        visualisations = new List<ScenarioSwitcher>(GetComponentsInChildren<ScenarioSwitcher>());
        altitudeChecks = new List<AltitudeCheck>(GetComponentsInChildren<AltitudeCheck>());
        scenarios = new List<string>();
        scenarioObjects = new Dictionary<string, List<GameObject>>();
        //TODO: Load scenario names from directory
        //for now use hard coded names
        scenarios.Add("Scenario1");
        scenarios.Add("Scenario2");

        foreach (string scenario in scenarios)
        {
            initialiseData(scenario);
        }
    
        // StartCoroutine(createVisualisations());
        initialiseAltitudePosition();
        setDesiredAltitude();

        

        //Default to first scenario
        SetActiveScenario(scenarios[0]);

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
            file.SetRocketId("Rocket" + (i + 1));

            file.initialiseDataSet();
            //Dynamically find the min and max of each dimension across all datasets
            //Needed to normalise all the data to the same scale for comparison
            file.GetMinMax();
            if (globalMin == null)
            {
                globalMin = file.dimensionMin;
            }
            if (globalMax == null)
            {
                globalMax = file.dimensionMax;
            }

            for (int j = 0; j < file.dimensionMin.Length; j++)
                if (file.dimensionMin[j] < globalMin[j])
                    globalMin[j] = file.dimensionMin[j];

            for (int j = 0; j < file.dimensionMax.Length; j++)
                if (file.dimensionMax[j] > globalMax[j])
                    globalMax[j] = file.dimensionMax[j];
            //scenarioObj.SetActive(false);
        }

        scenarioObjects.Add(scenario, scenarioList);


        // foreach (DataFiles file in files)
        // {
        //     file.initialiseDataSet();
        //     //Dynamically find the min and max of each dimension across all datasets
        //     //Needed to normalise all the data to the same scale for comparison
        //     file.GetMinMax();
        //     if (globalMin == null)
        //     {
        //         globalMin = file.dimensionMin;
        //     }
        //     if (globalMax == null)
        //     {
        //         globalMax = file.dimensionMax;
        //     }

        //     for (int i = 0; i < file.dimensionMin.Length; i++)
        //         if (file.dimensionMin[i] < globalMin[i])
        //             globalMin[i] = file.dimensionMin[i];

        //     for (int i = 0; i < file.dimensionMax.Length; i++)
        //         if (file.dimensionMax[i] > globalMax[i])
        //             globalMax[i] = file.dimensionMax[i];
        // }
    }

    private IEnumerator createVisualisations()
    {
        foreach (DataFiles file in files)
        {
            //Set the min and max of each datafile to the global min and max so it can all be normalised to the same scale
            file.dimensionMin = globalMin;
            file.dimensionMax = globalMax;
            file.setSimulationFilesCoroutine();
        }
        yield return null;
    }

    private void setDesiredAltitude()
    {
        foreach (AltitudeCheck altitudeCheck in altitudeChecks)
        {
            altitudeCheck.setHeight(globalMax[2], globalMin[2]);
        }
    }

    private void initialiseAltitudePosition()
    {
        foreach (AltitudeCheck altitudeCheck in altitudeChecks)
        {
            altitudeCheck.Initialise();
        }
    }

    //Change the current scenario to the specified index in the scenarios array
    public void ChangeScenario(int scenario)
    {
            SetActiveScenario(scenarios[scenario]);
    }

    private void SetActiveScenario(string scenario)
    {
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
                    file.dimensionMin = globalMin;
                    file.dimensionMax = globalMax;
                    file.setSimulationFilesCoroutine();
                }                
            }
        }

        currentScenario = scenario;
    }
}
