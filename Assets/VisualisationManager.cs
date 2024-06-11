using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

public class VisualisationManager : MonoBehaviour
{
    private List<Visualisation> visualisations;
    private List<DataFiles> files;
    private List<string> scenarios;
    private string currentScenario;

    private float[] globalMin;
    private float[] globalMax;

    // Start is called before the first frame update
    void Start()
    {
        files = new List<DataFiles>(GetComponentsInChildren<DataFiles>());

        initialiseData();

        createVisualisations();

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
            file.DestroyTrajectories();
            file.setScenario(currentScenario);
        }
            
        initialiseData();

        createVisualisations();
    }

    private void initialiseData()
    {
        foreach (DataFiles file in files)
        {
            file.initialiseDataSet();

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

    private void createVisualisations()
    {
        foreach (DataFiles file in files)
        {
            file.dimensionMin = globalMin;
            file.dimensionMax = globalMax;
            file.setSimulationFilesCoroutine();
        }
    }

    public void ChangeScenario(int scenario)
    {
        currentScenario = scenarios[scenario];
        RemakeVisualisations();
    }
}
