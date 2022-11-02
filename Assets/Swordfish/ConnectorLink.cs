using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates a line renderer between two GameObject points
[RequireComponent(typeof(LineRenderer))]
public class ConnectorLink : MonoBehaviour
{
    public GameObject anchor;

    private bool lineSet = false;
    private Vector3 other;
    private Vector3 anchorLastPos;
    private LineRenderer line;
    private void Update()
    {
        if (lineSet && line.GetPosition(1) != anchor.transform.position)
        {
            line.SetPosition(0, other);
            line.SetPosition(1, anchor.transform.position);
        }
    }
    public void SetPointA(Vector3 pos)
    {
        line = GetComponent<LineRenderer>();
        line.SetPosition(0, pos);
        line.SetPosition(1, anchor.transform.position);
        lineSet = true;
        other = pos;
    }
}
