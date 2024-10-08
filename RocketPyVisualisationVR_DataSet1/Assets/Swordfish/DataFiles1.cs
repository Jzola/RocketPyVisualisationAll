using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using System.IO;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;

public class DataFilesSingle : DataFiles
{
    private float colourCounter = 0;

    private Material[] dataPointMats;

    // Creates trajectory data objects (BigMesh, LineRenderer, MeshCollider)
    protected override void CreateTrajectory(int fileIndex)
    {
        Color colour = Color.white;
        // Create the Visualisation object for respective trajectory.
        Visualisation visualisation = Instantiate(visualisationPrefab, transform).GetComponent<Visualisation>();
        visualisation.dataSource = files[fileIndex];
        switch (scenario)
        {
            case "Scenario1":
                colour = new Color(1, 1 - colourCounter, 1 - colourCounter, 1);
                visualisation.colour = colour;
                colourCounter += .033f;
                break;
            case "Scenario2":
                if (fileIndex < 10)
                {
                    colour = new Color(0.3f - colourCounter, 0.3f - colourCounter, 1, 1);
                    visualisation.colour = colour;
                }
                else if (fileIndex < 20)
                {
                    colour = new Color(0.3f - colourCounter, 1, 0.3f - colourCounter, 1);
                    visualisation.colour = colour;
                }
                else if (fileIndex < 30)
                {
                    colour = new Color(1, 0.5f, 0.3f - colourCounter, 1);
                    visualisation.colour = colour;
                }
                else if (fileIndex < 40)
                {
                    colour = new Color(1, 0.3f - colourCounter, 0.3f - colourCounter, 1);
                    visualisation.colour = colour;
                }
                colourCounter += .03f;
                if (fileIndex > 0 && (fileIndex + 1) % 10 == 0)
                    colourCounter = 0;
                break;
            default:
                break;
        }
        visualisation.CreateVisualisation(AbstractVisualisation.VisualisationTypes.SCATTERPLOT);       
        BigMesh mesh = visualisation.theVisualizationObject.viewList[0].BigMesh;

        //files[fileIndex].transform.SetParent(visualisation.transform);

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = colour;

        // Create the VisualisationLine object for this trajectory
        GameObject line = new GameObject();
        line.SetActive(false);
        line.AddComponent<VisualisationLine>();
        line.GetComponent<VisualisationLine>().setVisualisationMesh(mesh);
        line.GetComponent<VisualisationLine>().setLineMaterial(mat);
        line.transform.SetParent(visualisation.transform, false);

        line.SetActive(true);
    }
}
