using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;
using IATK;
using System.IO;

public class SetupChart : MonoBehaviour
{
    private ScatterChart chart;
    private CSVDataSource dataSource;

    private void OnEnable()
    {
        
    }

    private void InitialiseChart()
    {
        chart = gameObject.GetComponent<ScatterChart>();

        chart.EnsureChartComponent<Title>().show = true;
        chart.EnsureChartComponent<Title>().text = "test";

        var grid = chart.EnsureChartComponent<GridCoord>();
        grid.bottom = 50;
        grid.right = 30;
        grid.left = 60;
        grid.top = 80;

        chart.RemoveChartComponent<VisualMap>();

        chart.RemoveData();

        chart.AddSerie<Scatter>();

        chart.GetSerie<Scatter>().symbol.size = 5;
    }

    public void LoadData(string xVar, string yVar)
    {
        InitialiseChart();

        string[] filePath = Directory.GetFiles(Application.dataPath + "/Resources/MainData", "*.csv");
        GameObject inputDataObj = new GameObject("InputData");
        inputDataObj.transform.SetParent(this.transform, false);
        inputDataObj.AddComponent<CSVDataSource>();
        TextAsset inputDataAsset = TextfromFile(filePath[0]);
        inputDataAsset.name = "InputData";

        // Load the simulation data into the csvDataSource object
        inputDataObj.GetComponent<CSVDataSource>().data = inputDataAsset;
        inputDataObj.GetComponent<CSVDataSource>().loadHeader();
        inputDataObj.GetComponent<CSVDataSource>().load();
        dataSource = inputDataObj.GetComponent<CSVDataSource>();

        int xCol = dataSource.findCol(xVar);
        int yCol = dataSource.findCol(yVar);

        float[] dataSetX = dataSource.GetCol(dataSource.dataArray, xCol);
        float[] dataSetY = dataSource.GetCol(dataSource.dataArray, yCol);
        (float, float) x_minmax = GetMinMax(dataSetX);
        (float, float) y_minmax = GetMinMax(dataSetY);

        chart.EnsureChartComponent<Title>().show = true;
        chart.EnsureChartComponent<Title>().text = yVar + " vs " + xVar;

        var xAxis = chart.EnsureChartComponent<XAxis>();
        var yAxis = chart.EnsureChartComponent<YAxis>();

        xAxis.axisName.show = true;
        xAxis.axisName.name = xVar;

        yAxis.axisName.show = true;
        yAxis.axisName.name = yVar;

        for (int i = 0; i < dataSetX.Length; i+=10)
        {
            chart.AddData(0, dataSetX[i], dataSetY[i]);
        }

        return;
    }
    private TextAsset TextfromFile(string path)
    {
        var sr = new StreamReader(path);
        string contents = sr.ReadToEnd();
        sr.Close();
        return new TextAsset(contents);
    }

    private (float, float) GetMinMax(float[] dataSet)
    {
        float min = dataSet[0];
        float max = dataSet[0];
        foreach (float value in dataSet)
        {
            if (value < min)
                min = value;

            if (value > max)
                max = value;
        }

        return (min, max);
    }
}
