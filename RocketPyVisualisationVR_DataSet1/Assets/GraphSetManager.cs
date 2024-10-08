using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphSetManager : MonoBehaviour
{
    [SerializeField]
    private GameObject singleGraphSetPrefab;
    [SerializeField]
    private GameObject multiGraphSetPrefab;

    private GameObject currentGraphSet;

    // Start is called before the first frame update
    void Start()
    {
        //SpawnMultiGraphSet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnSingleGraphSet()
    {
        spawnGraphSet(singleGraphSetPrefab);
    }

    public void SpawnMultiGraphSet()
    {
        spawnGraphSet(multiGraphSetPrefab);
        singleGraphSetPrefab.SetActive(false);
        VisualisationManager_S3 visualisationManager = multiGraphSetPrefab.GetComponentInChildren<VisualisationManager_S3>();
        visualisationManager.DrawGraphs();
    }

    private void spawnGraphSet(GameObject graphSet)
    {
        if (currentGraphSet != null)
            currentGraphSet.transform.position += new Vector3(0, -6, 0);

        graphSet.transform.position += new Vector3(0, 6, 0);
    }
}
