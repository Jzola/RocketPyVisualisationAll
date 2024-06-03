using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

public class VisualisationManager : MonoBehaviour
{
    private List<Visualisation> visualisations;
    private List<DataFiles> files;

    private float[] globalMin;
    private float[] globalMax;

    // Start is called before the first frame update
    void Start()
    {
        files = new List<DataFiles>(GetComponentsInChildren<DataFiles>());

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
}
