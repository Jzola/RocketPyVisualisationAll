using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

// Creates the points of a visualisation as separate GameObjects based on the IATK combined mesh
public class VisualisationPoints : MonoBehaviour
{
    // Prefab of object to instantiate the data point as
    private GameObject dataPointPrefab;

    private Vector3[] vertices;
    private List<GameObject> dataPoints;
    private BigMesh visualisationMesh;
    private Material pointMat;
    private bool pointsNeedUpdating = false;
    public bool tweenPointsOnUpdate = false;
    private bool pointsVisible = true;

    private Color[] classifications = new Color[]
    {
        new Color(0.396f, 0.45f, 0.745f),
        new Color(0.372f, 0.624f, 0.216f),
        new Color(1f, 1f, 1f),            // white for apogee
        new Color(0.576f, 0.29f, 0.8f),
        new Color(0.847f, 0.302f, 0.2f),
        new Color(0.651f, 0.475f, 0.192f),
        new Color(0.678f, 0.753f, 1f, 1f),
        new Color(0.898f, 0.773f, 0.071f, 1f)
    };

    private bool hasColouredApogee = false;
    private int apogeeColourIndex = 3;

    // Creates a GameObject dataPointPrefab consisted of individual objects representing
    // each data point from a CSV file.
    public void CreatePoints(float[] flightStageTimes, string colourCol, Material[] materials)
    {
        vertices = visualisationMesh.getBigMeshVertices();
        CSVDataSource dataSource = (CSVDataSource)GetComponentInParent<Visualisation>().dataSource;
        dataPoints = new List<GameObject>();

        // Instantiate separate GameObjects for each data point
        for (int i = 0; i < vertices.Length; i++)
        {
            GameObject pointGO = Instantiate(dataPointPrefab, Vector3.zero, Quaternion.identity);
            pointGO.transform.parent = gameObject.transform;
            pointGO.transform.localPosition = vertices[i];
            dataPoints.Add(pointGO);
            DataPoint point = pointGO.GetComponent<DataPoint>();

            // Set the data values for that point (i.e. the row of values from CSV)
            point.SetData(dataSource, dataSource.GetRow(dataSource.dataArray, i));

            // Add the correct material
            // If it is the final data point of the trajectory, set it to the impact material
            if (i == (vertices.Length-1))
            {
                point.GetComponent<MeshRenderer>().material = materials[materials.Length-1];
            }
            else
            {
                point.GetComponent<MeshRenderer>().material = materials[CalcColor(point, flightStageTimes, i, colourCol)];
            }
        }
    }

    // Updates the points already placed on the scene, moving them if the BigMesh has changed
    public void updatePoints()
    {
        // Updates the Visualisation Points vertices with the new mesh
        vertices = visualisationMesh.getBigMeshVertices();
        CSVDataSource dataSource = (CSVDataSource)GetComponentInParent<Visualisation>().dataSource;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Moves the data points
            if (!tweenPointsOnUpdate) dataPoints[i].transform.localPosition = vertices[i];
            DataPoint point = dataPoints[i].GetComponent<DataPoint>();

            // Set the data values for that point (i.e. the row of values from CSV)
            point.SetData(dataSource, dataSource.GetRow(dataSource.dataArray, i));
        }

        // Hides renderline while moving points
        transform.parent.GetComponentInChildren<VisualisationLine>().gameObject.GetComponent<Renderer>().enabled = !tweenPointsOnUpdate;
        pointsNeedUpdating = tweenPointsOnUpdate;
    }

    // Gets run every frame
    private void Update()
    {
        if (pointsNeedUpdating)
        {
            bool hasMoved = false;
            // For every known point, if its not where it should be, it is animated towards where it needs to be
            for (int i = 0; i < dataPoints.Count; i++)
            {
                float distance = Vector3.Distance(dataPoints[i].transform.localPosition, vertices[i]);
                float minDist = 0.01f;

                // Checks if points are where they should be, on the vertices
                if (Vector3.Distance(dataPoints[i].transform.localPosition, vertices[i]) > minDist)
                {
                    float speed = distance * 0.9f;
                    float minSpeed = 0.2f;

                    // Moves towards the target spot, based on given speed and time.
                    dataPoints[i].transform.localPosition = Vector3.MoveTowards(dataPoints[i].transform.localPosition, vertices[i], (speed > minSpeed ? speed : minSpeed) * Time.deltaTime);
                    hasMoved = true;
                } else
                {
                    // If the points are within the minimum distance, teleport to their exact position
                    dataPoints[i].transform.localPosition = vertices[i];
                }
            }

            if (!hasMoved)
            {
                pointsNeedUpdating = false;
                transform.parent.GetComponentInChildren<VisualisationLine>().gameObject.GetComponent<Renderer>().enabled = true;
            }
        } 
    }

    // Calculates the colour of data point based upon what stage of the flight
    // it exists within
    private int CalcColor(DataPoint point, float[] flightStageTimes, int index, string var)
    {
        // Get the time variable value from the data point
        int colIndex = point.getData().findCol(var);
        float time = point.getData().GetRow(point.getData().dataArray, index)[colIndex];

        // Find which stage the data point belongs in.
        // We are assuming the array is sorted ascending
        for (int i = 0; i < flightStageTimes.Length; i++)
        {
            if (time < flightStageTimes[i])
            {
                // TODO: find the explicit data point that is closest to apogee? rather
                // than colouring the first phase 3 data point.

                // Check if data point is the first phase 3 data point (therefore apogee)
                if (hasColouredApogee == false && i == (apogeeColourIndex+1))
                {
                    hasColouredApogee = true;
                    return i - 2;
                }
                else
                {
                    return i - 1;
                }
            }
        }
        return classifications.Length - 1;
    }

    // Calculates a the correct colour based upon the value of the 'var' column against the
    // max value in that column. It takes a gradient between 'start' and 'end' colours.
    private Color CalcGradient(DataPoint point, Color start, Color end, int index, string var)
    {
        int colIndex = point.getData().findCol(var);

        // If error in finding column value, return the start colour
        if (colIndex == -1)
        {
            return start;
        }

        float time = point.getData().GetRow(point.getData().dataArray, index)[colIndex];
        float maxTime = point.getData().getDimensions()[colIndex].MetaData.maxValue;

        // Calculate the colour based on current value against max value
        double pct = (double)time / (double)maxTime;
        float red = (float)(start.r + pct * (end.r - start.r));
        float green = (float)(start.g + pct * (end.g - start.g));
        float blue = (float)(start.b + pct * (end.b - start.b));

        return new Color(red, green, blue, 1);
    }

    public List<GameObject> DataPoints()
    {
        return dataPoints;
    }

    public void setDataPointPrefab(GameObject prefab)
    {
        dataPointPrefab = prefab;
    }

    public void setVisualisationMesh(BigMesh mesh)
    {
        visualisationMesh = mesh;
    }

    public void setPointMaterial(Material mat)
    {
        pointMat = mat;
    }
}