using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarGraph.VittorCloud;
using System;
using IATK;
using UnityEngine.UI;
using TMPro;

// Script on the bar GameObject with the collider
[RequireComponent(typeof(BoxCollider))]
public class Bar : MonoBehaviour
{
    private GameObject valuesDisplay;
    private bool selected = false;

    private String xValue;
    private String zValue;
    private int id;
    private String descriptiveSummary;
    private Vector3 topOfBar;
    private CSVDataSource fileData;
    private OutputVariableVisibility valueVisibility;

    // How many frames until display updates. Allows all bars to recieve update notice before reseting visibility flag.
    private int framesTillUpdate = -1;
    private int frameDelay = 2;

    // Formatting for richtext
    private string[][] f = { new string[]{"<b>", "</b>"},
                             new string[]{"<i>", "</i>"},
                             new string[]{"<u>", "</u>"},
                             new string[]{"<color=#707070>", "</color>"}};

    private void Start()
    {
        LoadInputVariables input = GetComponentInParent<LoadInputVariables>();
        fileData = input.GetDataSource()[0];
        id = GetComponentInParent<BarProperty>().GetBarId();

        // Get visibility from config
        valueVisibility = GetComponentInParent<ThirdLevelChartLinkingManager>().transform.parent.parent.GetComponentInChildren<OutputVariableVisibility>();

        //Ignore the collisions between layer 0 (default) and layer 8 (custom layer you set in Inspector window)
        Physics.IgnoreLayerCollision(0, 7);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    SelectionCollider cone = other.gameObject.GetComponent<SelectionCollider>();

    //    // Collided with controller cone
    //    if (cone)
    //    {
    //        ChartLinkingManager manager = GetComponentInParent<ChartLinkingManager>();
    //        manager.HighlightID(GetComponentInParent<BarProperty>().GetBarId());
    //    }
    //}

    private void Update()
    {

        // Connect the line renderer of the display to the top of the bar
        if (valuesDisplay && valuesDisplay.activeInHierarchy)
        {
            topOfBar = transform.parent.GetComponentInChildren<TextMeshPro>(true).transform.position;
            valuesDisplay.transform.position = topOfBar + new Vector3(0, 0.25f, 0);
            valuesDisplay.GetComponentInChildren<ConnectorLink>().SetPointA(topOfBar + new Vector3(0, -0.02f, 0));
        }


        // Handles a delayed update to the data display window
        framesTillUpdate--;
        if (framesTillUpdate == 0)
        {
            updateDisplay();
            valueVisibility.filterChanged = false;
        }
        if (selected && valueVisibility.filterChanged && framesTillUpdate < 0)
        {
            framesTillUpdate = frameDelay;
        }
    }

    [ContextMenu("Select")]
    public void SelectBar()
    {
        if (!valuesDisplay)
        {            
            // Instantiate UI display of values
            var valuesDisplayPrefab = Resources.Load<GameObject>("Prefabs/BarUIDisplay");


            // Get position of the top of the bar - which the text label is conveniently instatiated at that accounts 
            // differing heights of bars
            topOfBar = transform.parent.GetComponentInChildren<TextMeshPro>(true).transform.position;
            valuesDisplay = Instantiate(valuesDisplayPrefab, topOfBar + new Vector3(0, 0.25f, 0), transform.rotation);
            valuesDisplay.transform.parent = transform.parent.transform;

            updateDisplay();

            // Connect the line renderer of the display to the top of the bar
            valuesDisplay.GetComponentInChildren<ConnectorLink>().SetPointA(topOfBar + new Vector3(0, -0.1f, 0));
            selected = true;
        }
        else
        {
            if (selected)
            {
                valuesDisplay.SetActive(false);
                selected = false;
            }
               
            else
            {
                updateDisplay();
                valueVisibility.filterChanged = false;
                valuesDisplay.SetActive(true);
                selected = true;
            }                
        }
        
    }

    private void updateDisplay()
    {
        descriptiveSummary = "";

        // Build the string of the bar's information
        float[] idCol = fileData.GetCol(fileData.dataArray, 0);
        for (var i = 0; i < idCol.Length; i++)
        {
            if (idCol[i] == id)
            {
                float[] rowData = fileData.GetRow(fileData.dataArray, i);
                LoadInputVariables inputLoader = GetComponentInParent<LoadInputVariables>();

                // ID of trajectory
                descriptiveSummary += f[0][0] + fileData[inputLoader.idCol].Identifier + f[0][1] + ": " + rowData[inputLoader.idCol] + "\n";

                // y Axis variable
                int yColNum = fileData.findCol(inputLoader.axisY);
                // If attribute is stored as a String
                if (fileData[yColNum].MetaData.type == DataType.String)
                    // Get the original String of the attribute (because IATK normalises the original string to a number)
                    xValue = fileData.getOriginalString(yColNum, rowData[yColNum]);
                else
                    xValue = rowData[yColNum].ToString();
                descriptiveSummary += f[0][0] + fileData[yColNum].Identifier + f[0][1] + ": " + xValue + "\n";

                // X Axis variable
                int xColNum = fileData.findCol(inputLoader.axisX);
                // If attribute is stored as a String
                if (fileData[xColNum].MetaData.type == DataType.String)
                    // Get the original String of the attribute (because IATK normalises the original string to a number)
                    xValue = fileData.getOriginalString(xColNum, rowData[xColNum]);
                else
                    xValue = rowData[xColNum].ToString();
                descriptiveSummary += f[0][0] + fileData[xColNum].Identifier + f[0][1] + ": " + xValue + "\n";

                // 3D bar chart
                if (inputLoader.axisZ != "")
                {
                    // Z Axis variable
                    int zColNum = fileData.findCol(inputLoader.axisZ);
                    if (fileData[zColNum].MetaData.type == DataType.String)
                        zValue = fileData.getOriginalString(zColNum, rowData[zColNum]);
                    else
                        zValue = rowData[zColNum].ToString();
                    descriptiveSummary += f[0][0] + fileData[zColNum].Identifier + f[0][1] + ": " + zValue + "\n";
                }
                break;
            }
        }

        // Separator between abstracted variable(s) 
        descriptiveSummary += "--------\n";
        // and statistics of its output trajectory
        descriptiveSummary += GetDimensionsMinMax();
        
        // Set text to display values of data point
        valuesDisplay.GetComponentInChildren<Text>().text = descriptiveSummary;
        valuesDisplay.GetComponentInChildren<VerticalLayoutGroup>().enabled = false;
        valuesDisplay.GetComponentInChildren<VerticalLayoutGroup>().enabled = true;
    }

    // Get the minimum and maximum values of each output variable/column of data of the trajectory
    private string GetDimensionsMinMax()
    {
        string results = f[3][0] + f[1][0] + "variable:  min  |  max" + f[1][1] + f[3][1] + "\n";
        string barID = GetComponentInParent<BarProperty>().GetBarId().ToString();
        CSVDataSource trajectoryData = GetComponentInParent<ChartLinkingManager>().GetComponentInChildren<DataFiles>().GetFile(barID);

        for (int i = 0; i < trajectoryData.DimensionCount; i++)
        {
            // Stop index iteration if hidden var
            if (valueVisibility != null && !valueVisibility.getVisibility(i)) continue;

            string newLine = f[0][0] + trajectoryData[i].Identifier + ": " + f[0][1] +  
                trajectoryData[i].MetaData.minValue + f[0][0] + "  |  " + f[0][1] + trajectoryData[i].MetaData.minValue + "\n";

            results += newLine;
        }
        return results;
    }

    public string GetZValue()
    {
        return zValue;
    }

    public string GetXValue()
    {
        return xValue;
    }
}
