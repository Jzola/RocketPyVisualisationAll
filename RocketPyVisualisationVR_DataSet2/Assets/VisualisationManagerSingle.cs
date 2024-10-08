using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualisationManagerSingle : VisualisationManager
{
    private int currentGraph = 0;

    public void NextGraph()
    {
        visualisations[currentGraph].transform.position -= new Vector3(0, 5, 0);
        currentGraph++;
        visualisations[currentGraph].transform.position += new Vector3(0, 5, 0);
    }
    public void PrevGraph()
    {
        visualisations[currentGraph].transform.position -= new Vector3(0, 5, 0);
        currentGraph--;
        visualisations[currentGraph].transform.position += new Vector3(0, 5, 0);
    }

    public void ResetGraphs()
    {
        visualisations[currentGraph].transform.position -= new Vector3(0, 5, 0);
        currentGraph = 0;
        visualisations[currentGraph].transform.position += new Vector3(0, 5, 0);
    }
        
}
