using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using BarGraph.VittorCloud;
using UnityEngine.UI;

public class DataPoint : MonoBehaviour
{
    public Material hoverMaterial;
    public Material selectedMaterial;

    private GameObject valuesDisplay;

    private Single[] data;
    private CSVDataSource dataSource;
    private bool selected = false;

    private MeshRenderer meshRenderer;

    // Original material of DataPoint on instantiation
    private Material originalMaterial;
    // Material of DataPoint directly before it is changed by script
    private Material previousMaterial;

    private String rawValuesString;
    private String filteredValuesString;

    private ChartLinkingManager manager;
    private int trajectoryID;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originalMaterial = meshRenderer.material;

        manager = GetComponentInParent<ChartLinkingManager>();
        trajectoryID = Int32.Parse(dataSource.data.name);

        // Get values of data point as a string
        updateValuesString();

    }

    public void SetData(CSVDataSource source, Single[] data)
    {
        dataSource = source;
        this.data = data;
        updateValuesString();
    }

    public void updateValuesString()
    {
        OutputVariableVisibility valueVisibility = GetComponentInParent<VisualisationPoints>().valueVisibility;

        rawValuesString = "ID: " + dataSource.GetID() + "\n";
        filteredValuesString = "ID: " + dataSource.GetID() + "\n";

        for (var i = 0; i < data.Length; i++)
        {
            string dataLine = dataSource[i].Identifier + ": " + data[i] + "\n";

            // If the data has been toggled off, don't add it to the line
            if (valueVisibility == null || valueVisibility.getVisibilities().Length == 0 || valueVisibility.getVisibility(i))
            {
                filteredValuesString += dataLine;
            }
            rawValuesString += dataLine;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SelectionCollider cone = other.gameObject.GetComponent<SelectionCollider>();

        // Collided with controller 
        if (cone)
        {
            // Highlight trajectory and corresponding bar in bar chart, and move slider to match bar input value
            //manager.HighlightID(trajectoryID);
            if (!selected)
            {
                previousMaterial = meshRenderer.material;
                // Change to Hover colour
                meshRenderer.material = hoverMaterial;
            } else
            {
                // Selected DataPoints remain highlighted and not overridden by the chart linking highlighting
                if (manager.GetDoHighlighting())
                {
                    meshRenderer.material = selectedMaterial;
                }
            }            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        SelectionCollider cone = other.gameObject.GetComponent<SelectionCollider>();

        // Collided with controller cone
        if (cone)
        {
            if (!selected)
            {
                if (!manager.GetDoHighlighting())
                {
                    // Change back to original colour
                    meshRenderer.material = originalMaterial;
                } 
                else
                {
                    meshRenderer.material = previousMaterial;
                }
            } else
            {
                meshRenderer.material = selectedMaterial;
            }
        }
    }

    // TODO for testing on PC only
    private void OnMouseEnter()
    {
       // manager.HighlightID(trajectoryID);
    }

    private void OnMouseDown()
    {
        // manager.LockHighlighting(trajectoryID);
        Select();
    }

    [ContextMenu("Select DataPoint")]
    public void Select()
    {
        // Was selected, now deselect
        if (selected)
        {
            selected = false;
            meshRenderer.material = hoverMaterial;

            Destroy(valuesDisplay);
        }
        // Was not selected, now select
        else
        {
            selected = true;
            meshRenderer.material = selectedMaterial;

            updateValuesString();

            // Instantiate UI display of values
            var valuesDisplayPrefab = Resources.Load<GameObject>("Prefabs/DataPointUIDisplay");
            valuesDisplay = Instantiate(valuesDisplayPrefab, Vector3.zero, Quaternion.identity);
            valuesDisplay.transform.parent = this.transform;

            // Rotate this data point to look at camera
            Vector3 v = transform.position - GameObject.FindGameObjectWithTag("MainCamera").transform.position;
            Quaternion q = Quaternion.LookRotation(v);
            valuesDisplay.transform.rotation = q;
            valuesDisplay.transform.localPosition = new Vector3(0, 12, -5);

            // Set text to display values of data point
            valuesDisplay.GetComponentInChildren<Text>().text = filteredValuesString;
            // Connect the line renderer of the display to the data point
            valuesDisplay.GetComponentInChildren<ConnectorLink>().SetPointA(transform.position);
        }                
    }

    public String GetRawValuesAsString()
    {
        return rawValuesString;
    }
    public String GetFilteredValuesAsString()
    {
        return filteredValuesString;

    }

    public void ResetMaterial()
    {
        meshRenderer.material = originalMaterial;
    }

    public Material GetOriginalMaterial()
    {
        return originalMaterial;
    }

    public int GetTrajectoryID()
    {
        return trajectoryID;
	}
	
    public void SetMaterial(Color colour)
    {
        meshRenderer.material.color = colour;
    }

    public CSVDataSource getData()
    {
        return dataSource;
    }
}
