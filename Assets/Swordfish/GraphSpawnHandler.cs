using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSpawnHandler : MonoBehaviour
{
    // Graphs
    private List<GameObject> graphs;
    private List<Vector3> graphPos;

    // Spawn Circles
    public GameObject spawnCirclePrefab;
    private List<GameObject> spawnCircles;
    private GameObject circlesContainer;
    private bool cirUpdateNeeded = true;
    private Color[] cirStateColours = { new Color(0.26f, 0.26f, 0.26f),     // Default
                                        new Color(0.59f, 0.81f, 0.66f),     // Selected
                                        new Color(0.36f, 0.52f, 0.66f) };   // In Use

    // Spawning
    private float radius = 5;
    private int maxGraphsInCircle = 6;
    public float spawnHeight;
    private Vector3 spawnCentre;
    private int nextSpawnIndex = 0;

    // Movement
    public bool animatedMove = true;
    private bool posUpdateNeeded = false;
    private float moveSpeed = 5;
    private float maxAnglePerSec = 90;
    private float snappingDist = 0.01f;

    // Awake is called when initilised
    void Awake()
    {
        graphs = new List<GameObject>();
        graphPos = new List<Vector3>();
        spawnCircles = new List<GameObject>();
        circlesContainer = new GameObject("SpawnCircles");
    }

    // Start is called during the first frame
    void Start()
    {
        createSpawnCircles();
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if any of the graphs were destroyed, updating if so
        for (int i = 0; i < graphs.Count; i++)
        {
            if (graphs[i] == null)
            {
                graphs.RemoveAt(i--);
                posUpdateNeeded = true;
                cirUpdateNeeded = true;
                nextSpawnIndex = -1;
            }
        }

        // Shifts graphs when one gets deleted
        if (posUpdateNeeded)
        {
            bool changeOccured = false;
            for (int i = 0; i < graphs.Count; i++)
            {
                // If the graph is far from its target position, move it part way.
                if (Vector3.Distance(graphs[i].transform.position, graphPos[i]) > snappingDist)
                {
                    // Calculating amount to move around the arc for the frame
                    float targetAngle = Vector3.Angle(graphs[i].transform.position - spawnCentre, graphPos[i] - spawnCentre);
                    float moveAngle = (targetAngle * moveSpeed < maxAnglePerSec ? targetAngle * moveSpeed : maxAnglePerSec) * Time.deltaTime;
                    Vector3 newPos;

                    if (moveAngle < 270) {
                        // Update position
                        Vector3 relPosCurrent = graphs[i].transform.position - spawnCentre;                     // Get current pos relative to the center
                        Vector3 relDirTarget = Quaternion.AngleAxis(-moveAngle, Vector3.up) * relPosCurrent;    // Get the direction to the target from centre
                        newPos = spawnCentre + relDirTarget.normalized * radius;                                // Add radius to direction from center, and set graph pos
                    } 
                    else
                    {
                        // If the graph somehow missed the target, then teleport to target
                        newPos = graphPos[i];
                    }

                    graphs[i].transform.position = newPos;
                    changeOccured = true;
                } 
                else
                {
                    // Snap graph onto target position
                    graphs[i].transform.position = graphPos[i];
                }
                // Aim graph at centre
                graphs[i].transform.LookAt(new Vector3(spawnCentre.x, graphs[i].transform.position.y, spawnCentre.z));
            }

            // If a change occured, update again next frame. Otherwise stop updating.
            posUpdateNeeded = changeOccured;
        }

        // Auto selects the next spawn spot if not already selected
        if (hasFreeSpace() && nextSpawnIndex == -1)
        {
            nextSpawnIndex = graphs.Count();
        }

        // Updates circle states
        if (cirUpdateNeeded)
        {
            // Sets in-use and default circles
            for (int i = 0; i < maxGraphsInCircle; i++)
            {
                SpawnCircle sc = spawnCircles[i].GetComponent<SpawnCircle>();
                // If graph exists on circle, set to in use, otherwise set to default.
                sc.changeState(graphs.Count > i && graphs[i] != null ? SpawnCircle.State.IN_USE : SpawnCircle.State.DEFAULT);
            }

            // If no circles selected, select next available space.
            if (nextSpawnIndex == -1 && hasFreeSpace()) nextSpawnIndex = graphs.Count();

            // Change state of selected circle to selected
            if (nextSpawnIndex != -1) spawnCircles[nextSpawnIndex].GetComponent<SpawnCircle>().changeState(SpawnCircle.State.SELECTED);

            cirUpdateNeeded = false;
        }
    }

    // Tries to add a graph to the handler. If max graphs have been hit, then returns false, otherwise true.
    public bool add(GameObject graph)
    {
        if (hasFreeSpace() || validSpotSelected()) {
            // Remove existing graph if spot is already occupied and put new graph in its spot
            if (graphs.Count() > nextSpawnIndex && graphs[nextSpawnIndex] != null)
            {
                Destroy(graphs[nextSpawnIndex]);
                graphs[nextSpawnIndex] = graph;
            } 
            else
            {
                // Add graph normally
                graphs.Add(graph);
            }

            // Set position and rotation
            //graph.transform.position = graphPos[graphs.IndexOf(graph)];
            graph.transform.position = graphPos[nextSpawnIndex];
            graph.transform.LookAt(new Vector3(spawnCentre.x, graph.transform.position.y, spawnCentre.z));

            posUpdateNeeded = true;
            flagCircleUpdate();
            return true;
        }
        return false;
    }

    // Remove a graph from the handler, destroying it
    public void remove(GameObject graph)
    {
        // If the graph is successfully removed, destroy it and trigger an update.
        if (graphs.Remove(graph))
        {
            Destroy(graph);
            posUpdateNeeded = true;
            flagCircleUpdate();
        }
    }

    // Removes and destroys all graphs from the handler
    public void clear()
    {
        foreach (GameObject graph in graphs)
        {
            Destroy(graph);
        }
        graphs.Clear();
        flagCircleUpdate();
    }

    public void setSpawnCentre(Vector3 pos)
    {
        spawnCentre = pos;
        updateSpawnLocations();
    }

    public void offsetSpawnCentre(Vector3 offset)
    {
        spawnCentre += offset;
        updateSpawnLocations();
    }

    public void setSpawnHeight(float graphHeight)
    {
        spawnHeight = graphHeight;
        updateSpawnLocations();
    }

    // Updates list of all potential spawn locations, based on field variables
    private void updateSpawnLocations()
    {
        graphPos.Clear();

        for (int i = 0; i < maxGraphsInCircle; i++)
        {
            float ang = i * (360 / maxGraphsInCircle);
            
            Vector3 pos = new Vector3();
            pos.x = spawnCentre.x + (radius * Mathf.Sin(ang * Mathf.Deg2Rad));
            pos.y = spawnCentre.y + spawnHeight;
            pos.z = spawnCentre.z + (radius * Mathf.Cos(ang * Mathf.Deg2Rad));

            graphPos.Add(pos);
        }

        posUpdateNeeded = true;
        createSpawnCircles();
    }

    // Returns a bool indicating whether there is a free space for a new graph or not
    public bool hasFreeSpace()
    {
        return graphs.Count < maxGraphsInCircle;
    }

    // Returns a bool indicating a spot has been selected
    public bool validSpotSelected()
    {
        return nextSpawnIndex >= 0;
    }

    // Creates graph spawn circles below the graph spawn positions
    private void createSpawnCircles()
    {
        // Remove old circles
        foreach(GameObject circle in spawnCircles)
        {
            Destroy(circle);
        }
        spawnCircles.Clear();

        foreach(Vector3 pos in graphPos)
        {
            // Set spawn height to feet
            Vector3 circlePos = new Vector3(pos.x, spawnCentre.y, pos.z);
            
            // Create circle and add it to circle list
            GameObject circle = Instantiate(spawnCirclePrefab, circlePos, spawnCirclePrefab.transform.rotation, circlesContainer.transform);
            spawnCircles.Add(circle);
            
            // Set field variables
            circle.GetComponent<SpawnCircle>().ID = spawnCircles.Count - 1;
            circle.GetComponent<SpawnCircle>().handler = this;
            circle.GetComponent<SpawnCircle>().colours = cirStateColours;
        }

        flagCircleUpdate();
    }

    public void selectCircle(int ID)
    {
        nextSpawnIndex = ID;
        cirUpdateNeeded = true;
    }

    // Enforces an update to spawn circles next frame
    private void flagCircleUpdate()
    {
        nextSpawnIndex = -1;
        cirUpdateNeeded = true;
    }
}
