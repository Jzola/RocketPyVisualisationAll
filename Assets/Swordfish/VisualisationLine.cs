using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

// Creates the points of a visualisation as Line based on the IATK combined mesh
[RequireComponent(typeof(LineRenderer))]
public class VisualisationLine : MonoBehaviour
{
    private Vector3[] vertices;
    private BigMesh visualisationMesh;
    private LineRenderer line;
    private Material lineMat;

    void Start()
    {
        createLineRenderer();
    }

    private void createLineRenderer()
    {
        vertices = visualisationMesh.getBigMeshVertices();

        // Creat the lineRenderer object from the BigMesh data
        line = GetComponent<LineRenderer>();
        line.startWidth = (0.006f);
        line.useWorldSpace = false;
        line.positionCount = vertices.Length;
        line.SetPositions(vertices);
        line.material = lineMat;
    }

    public void setVisualisationMesh(BigMesh mesh)
    {
        visualisationMesh = mesh;
        createLineRenderer();
    }
    
    public void setLineMaterial(Material mat)
    {
        lineMat = mat;
    }

    public void resetLineMaterialToOriginal()
    {
        line.material = lineMat;
    }
}
