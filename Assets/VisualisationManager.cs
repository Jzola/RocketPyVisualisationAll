using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using System;

public class VisualisationManager : MonoBehaviour
{
    private List<AltitudeCheck> altitudeChecks;
    private List<DataFiles> files;
    private List<string> scenarios;
    private string currentScenario;

    private float[] globalMin;
    private float[] globalMax;

    // Start is called before the first frame update
    void Start()
    {
        files = new List<DataFiles>(GetComponentsInChildren<DataFiles>());
        altitudeChecks = new List<AltitudeCheck>(GetComponentsInChildren<AltitudeCheck>());

        initialiseData();

        StartCoroutine(createVisualisations());
        initialiseAltitudePosition();
        setDesiredAltitude();

        scenarios = new List<string>();
        //TODO: Load scenario names from directory
        //for now use hard coded names
        scenarios.Add("Scenario1");
        scenarios.Add("Scenario2");

        //Default to first scenario
        currentScenario = scenarios[0];

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
            
        initialiseData();

        StartCoroutine(createVisualisations());
        setDesiredAltitude();
    }

    private void initialiseData()
    {
        foreach (DataFiles file in files)
        {
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

            for (int i = 0; i < file.dimensionMin.Length; i++)
                if (file.dimensionMin[i] < globalMin[i])
                    globalMin[i] = file.dimensionMin[i];

            for (int i = 0; i < file.dimensionMax.Length; i++)
                if (file.dimensionMax[i] > globalMax[i])
                    globalMax[i] = file.dimensionMax[i];
        }
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
        currentScenario = scenarios[scenario];
        RemakeVisualisations();
    }
}
