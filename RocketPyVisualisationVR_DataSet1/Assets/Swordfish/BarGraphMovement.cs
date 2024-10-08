using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarGraph.VittorCloud;

// Script added onto GraphBox prefab
public class BarGraphMovement : MonoBehaviour
{
    private BarGraphManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<BarGraphManager>();
        //init = false;
        Vector3 xMiddle = manager.XAxis.GetComponentInChildren<Renderer>().bounds.center;
        Vector3 zMiddle = manager.ZAxis.GetComponentInChildren<Renderer>().bounds.center;
        Vector3 yMiddle = manager.YAxis.GetComponentInChildren<Renderer>().bounds.center;
        GameObject anchor = new GameObject("BarGraphAnchor");
        anchor.transform.position = new Vector3(xMiddle.x, xMiddle.y, zMiddle.z);
        anchor.transform.parent = this.transform.parent;
        this.transform.parent = anchor.transform;

        GetComponentInChildren<RotateParent>().transform.parent = anchor.transform;
    }
}
