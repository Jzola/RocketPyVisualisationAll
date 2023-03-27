using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphMenuSetCreator : MonoBehaviour
{

    public GameObject GraphPrefab;
    private List<GameObject> createdGraphs;

    // Start is called before the first frame update
    void Start()
    {
        createdGraphs = new List<GameObject>();
        CreateGraph();
    }

    [ContextMenu("Create Graph")]
    // Creates a graph, based on the given prefab
    public void CreateGraph()
    {
        createdGraphs.Add(Instantiate(GraphPrefab));
    }

    [ContextMenu("Delete Last Graph")]
    public void DeleteLastGraph()
    {
        if (createdGraphs.Count < 1) return;

        GameObject graph = createdGraphs[createdGraphs.Count - 1];
        createdGraphs.Remove(graph);
        Destroy(graph);
    }
}
