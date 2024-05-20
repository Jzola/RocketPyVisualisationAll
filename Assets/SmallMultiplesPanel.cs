using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SmallMultiplesPanel : MonoBehaviour
{
    [SerializeField]
    private Button spawnGraphButton;
    [SerializeField]
    private GameObject graphPrefab;
    [SerializeField]
    private GameObject variableSelection;
    [SerializeField]
    private GameObject spawnArea;
    [SerializeField]
    private Button clearGraphButton;

    private string xVar;
    private string yVar;
    private List<GameObject> graphs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        spawnGraphButton.onClick.AddListener(delegate { spawnGraph(); });
        clearGraphButton.onClick.AddListener(delegate { clearGraphs(); });
    }

    private void spawnGraph()
    {
        VariableSelection outputVariables = variableSelection.GetComponentInChildren<VariableSelection>();
        List<string> selectedVariables = outputVariables.GetSelection();
        xVar = selectedVariables[0];
        foreach (string variable in selectedVariables.Skip(1))
        {
            yVar = variable;
            GameObject smallMultipleGraph = Instantiate(graphPrefab, spawnArea.transform.position, Quaternion.identity, spawnArea.transform);
            graphs.Add(smallMultipleGraph);
            SetupChart chart = smallMultipleGraph.GetComponentInChildren<SetupChart>();
            chart.LoadData(xVar, yVar);
        }

        outputVariables.ClearSelection();
    }

    private void clearGraphs()
    {
        foreach (GameObject graph in graphs)
        {
            graph.Destroy();
        }
    }
}
