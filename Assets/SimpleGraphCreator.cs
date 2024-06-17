using IATK;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using BarGraph.VittorCloud;

public class SimpleGraphCreator : GraphCommon
{
    //Graph prefab is based on the original Input data using different rocket types and weights.
    public GameObject scatterGraphPrefab;

    private GraphSpawnHandler graphHandler;

    [Header("Object to Spawn Around")]
    public GameObject VRCamera;
    Vector3 VRCamOriginalPosition; // = VRCamera.transform.position;
    public GameObject spawnCirclePrefab;

    // Start is called before the first frame update
    void Start()
    {
        Initialise();
        graphHandler = gameObject.AddComponent<GraphSpawnHandler>();
        graphHandler.spawnCirclePrefab = spawnCirclePrefab;
        graphHandler.setSpawnCentre(VRCamera.transform.position);

        //force the height to where it is visible to player
        float graphHeight = 1.5f;
        graphHandler.setSpawnHeight(graphHeight);
    }

    // Returns bool whether there is a free space in the graph handler for more graphs or not
    public bool hasFreeSpace()
    {
        return graphHandler.hasFreeSpace();
    }

    public bool canCreateNewGraph()
    {
        return graphHandler.hasFreeSpace() && graphHandler.validSpotSelected();
    }

    [ContextMenu("Create Graph")]
    // Creates a graph, based on the given prefab and set axes
    public void CreateGraph()
    {
        if (!graphHandler.hasFreeSpace()) return;

        GameObject graph = null;

        graph = Instantiate(scatterGraphPrefab);
        setGraphAxisVariables(graph);

        graphHandler.add(graph);
    }
}
