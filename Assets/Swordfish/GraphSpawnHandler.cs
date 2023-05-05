using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphSpawnHandler : MonoBehaviour
{
    private List<GameObject> graphs;
    private List<Vector3> graphPos;

    // Spawning
    private float radius = 5;
    private float maxGraphsInCircle = 6;
    private float spawnHeight = 0.5f;
    private Vector3 spawnCentre;

    // Movement
    public bool animatedMove = true;
    private bool updateNeeded = false;
    private float moveSpeed = 5;
    private float maxAnglePerSec = 90;
    private float snappingDist = 0.01f;

    // Awake is called when initilised
    void Awake()
    {
        graphs = new List<GameObject>();
        graphPos = new List<Vector3>();

        setSpawnCentre(new Vector3(0,0,0));
        updateSpawnLocations();
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
                updateNeeded = true;
            }
        }

        if (updateNeeded)
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
            updateNeeded = changeOccured;
        }
    }

    // Tries to add a graph to the handler. If max graphs have been hit, then returns false, otherwise true.
    public bool add(GameObject graph)
    {
        if (graphs.Count != maxGraphsInCircle) {
            // Add graph, and set position and rotation
            graphs.Add(graph);
            graph.transform.position = graphPos[graphs.IndexOf(graph)];
            graph.transform.LookAt(new Vector3(spawnCentre.x, graph.transform.position.y, spawnCentre.z));
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
            updateNeeded = true;
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

        updateNeeded = true;
    }

    public bool hasFreeSpace()
    {
        return graphs.Count < maxGraphsInCircle;
    }
}
