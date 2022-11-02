using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using System.IO;
using BarGraph.VittorCloud;
using System;
using System.Linq;

public class LoadInputVariables : MonoBehaviour
{
    // Data objects
    private List<CSVDataSource> fileData = new List<CSVDataSource>();
    private List<BarGraphDataSet> barData = new List<BarGraphDataSet>();
    [SerializeField]
    private DataFiles trajectoryFiles;

    // Simulation Data
    private string folder = "MainData/inputData/";
    private string path = "/Resources/";
    public int idCol = 0;

    // Axes info
    public string axisX = "rocketMass (kg)";
    public string axisY = "time (s)";
    public string axisZ = "motor";

    // Graph object settings
    private BarGraphGenerator barGraph;
    [SerializeField]
    private int graphFactor = 1;
    [SerializeField]
    private float graphSize = 0.164f;
    [SerializeField]
    private Color barColor = new Color(1, 0, 0);

    private void Start()
    {
        // Load the csv data and create the BarGraphDataSet objects
        CreateData();

        // Create the graph generator object and generate the graph
        barGraph = GetComponent<BarGraphGenerator>();
        if (axisZ == "")
        {
            barGraph.ThreeDimensions(false);
        }
        barGraph.GeneratBarGraph(barData);

        // Decrease the graph size to reasonable level
        float scale = graphSize * graphFactor;
        this.transform.localScale = new Vector3(scale, scale, scale);

        if (trajectoryFiles != null)
        {
            SetVisKey();
        }
    }

    // Loads the csv file and creates csvDataSource and BarGraphDataSet objects
    // containing the x/y axis data for each row.
    private void CreateData()
    {
        // Load the file and create csvDataSource Objects
        try
        {
            string[] filePath = Directory.GetFiles(Application.dataPath + (path+folder), "*.csv");

            // Input data should be in a single csv file, as such we can load from index 0
            // Create new game object with CSVDataSource component
            GameObject dataSourceObj = new GameObject("DataSource");
            dataSourceObj.transform.SetParent(this.transform, false);
            dataSourceObj.AddComponent<CSVDataSource>();

            // Set CSVDataSource data to file data.
            TextAsset data = TextfromFile(filePath[0]);

            // Get the file name of the file and set as data name
            // For identification in ChartLinkingManager
            data.name = filePath[0].Substring(filePath[0].LastIndexOf("\\") + 1, filePath[0].LastIndexOf(".") - filePath[0].LastIndexOf("\\") - 1);
            dataSourceObj.GetComponent<CSVDataSource>().data = data;
            dataSourceObj.GetComponent<CSVDataSource>().loadHeader();
            dataSourceObj.GetComponent<CSVDataSource>().load();
            fileData.Add(dataSourceObj.GetComponent<CSVDataSource>());

            int xColNum = fileData[0].findCol(axisX);
            int uniqueXVals = fileData[0].GetCol(fileData[0].dataArray, xColNum).Distinct<float>().Count<float>();
            int yColNum = fileData[0].findCol(axisY);


            // If bar chart is to be three dimensions
            if (axisZ != "")
            {
                // Get information about the z-column data
                int zColNum = fileData[0].findCol(axisZ);
                float[] zCol = fileData[0].GetCol(fileData[0].dataArray, zColNum);
                float[] uniqueZVals = zCol.Distinct<float>().ToArray<float>();

                // Loop for number of unique z-axis values
                // Each loop creates a row of z-axis bars
                for (int i = 0; i < uniqueZVals.Count<float>(); i++)
                {
                    // The row indices for a given z-axis value
                    List<int> indices = fileData[0].getRowIndices(zColNum, uniqueZVals[i]);
                    Color barColor = new Color(1, 0.15f + (i * 0.1f), 0);

                    // Create one dimension of data
                    string name = fileData[0].getOriginalString(zColNum, fileData[0].GetRow(fileData[0].dataArray, indices[0])[zColNum]);
                    BarGraphDataSet oneDim = CreateOneDim(
                        name,
                        barColor,
                        fileData[0].getDimensions()[xColNum].Identifier,
                        fileData[0].getDimensions()[yColNum].Identifier,
                        fileData[0].getDimensions()[zColNum].Identifier,
                        uniqueXVals,
                        xColNum,
                        yColNum,
                        indices
                    );
                    barData.Add(oneDim);
                }
            }
            // Two dimensions
            else
            {
                // As this is only x/y-axis, we can loop through the data in one go.
                List<int> indices = Enumerable.Range(0, fileData[0].DataCount).ToList();

                string name = fileData[0].getDimensions()[xColNum].Identifier;
                BarGraphDataSet oneBar = CreateOneDim(
                    name,
                    barColor,
                    fileData[0].getDimensions()[xColNum].Identifier,
                    fileData[0].getDimensions()[yColNum].Identifier,
                    "", // 2D graph so no z-axis
                    fileData[0].DataCount,
                    xColNum,
                    yColNum,
                    indices
                );
                barData.Add(oneBar);
            }
        }
        catch (IOException e)
        {
            Debug.Log("Error loading input variable data: " + e);
        }
    }

    // Creates a single dimension of data objects.
    // For IBC package, a single dimension is the z-axis. Therefore this method creates all
    // objects for a single z-axis value.
    private BarGraphDataSet CreateOneDim(string name, Color color, string axisX, string axisY, string axisZ, int numXPoints, int xColNum, int yColNum, List<int> rowIndices)
    {
        // Create Z dimension object
        BarGraphDataSet oneDim = new BarGraphDataSet()
        {
            GroupName = name,
            barColor = color,
            ListOfBars = new List<XYBarValues>(),
            axisX = axisX,
            axisY = axisY,
            axisZ = axisZ,
        };

        // Create the BarGraphDataSet objects
        for (int i = 0; i < numXPoints; i++)
        {
            // Get the current row of data
            float[] row = fileData[0].GetRow(fileData[0].dataArray, rowIndices[i]);

            // Create the XYBarValues and add to object
            XYBarValues values = new XYBarValues()
            {
                XValue = Math.Round(row[xColNum], 2).ToString(),
                YValue = (float)Math.Round(row[yColNum], 2),
                id = (int)row[idCol],
            };

            oneDim.ListOfBars.Add(values);
        }
        // Sort and return z-dimension
        oneDim.ListOfBars.Sort();
        return oneDim;
    }

    // Creates TextAsset object from a filepath
    private TextAsset TextfromFile(string path)
    {
        var sr = new StreamReader(path);
        string contents = sr.ReadToEnd();
        sr.Close();
        return new TextAsset(contents);
    }

    // Set the trajectory visualisation key to the launch latitude and longitude
    private void SetVisKey()
    {
        int latCol = fileData[0].findCol("latitude");
        int lonCol = fileData[0].findCol("longitude");
        float[] row = fileData[0].GetRow(fileData[0].dataArray, 0);

        float lat = row[latCol];
        float lon = row[lonCol];

        trajectoryFiles.SetKey(lat, lon);
    }

    public List<CSVDataSource> GetDataSource()
    {
        return fileData;
    }

    public List<BarGraphDataSet> GetBarDataSets()
    {
        return barData;
    }

    // Check if this is a three dimensional bar chart
    public bool Is3D()
    {
        if (axisZ != "")
        {
            return true;
        }
        return false;
    }

    // Returns an entire row of bar objects. (i.e. returns all bars for a single z-axis value) - TODO Not used
    public List<XYBarValues> GetRowZ(int row)
    {
        if (!Is3D())
        {
            return null;
        }
        return barData[row].ListOfBars;
    }

    // Returns an entire column of bar objects. (i.e. returns all bars for a single x-axis value) - TODO Not used
    public List<XYBarValues> GetColX(int col)
    {
        List<XYBarValues> rowX = new List<XYBarValues>();

        // Get correct bar object from each z-axis.
        for (int i = 0; i < barData.Count; i++)
        {
            rowX.Add(barData[i].ListOfBars[col]);
        }

        return rowX;
    }
}
