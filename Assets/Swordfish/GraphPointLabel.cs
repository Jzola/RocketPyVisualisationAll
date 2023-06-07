using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graph.VittorCloud;

public class GraphPointLabel : MonoBehaviour
{
    public enum AxisType
    {
        X,
        Z
    }
    public AxisType axisType;
    private GraphPoint point;
    private ThirdLevelChartLinkingManager manager;
    private void Start()
    {
        point = GetComponentInParent<GraphPoint>();
        manager = GetComponentInParent<ThirdLevelChartLinkingManager>();
    }
    private void OnMouseEnter()
    {
        //if (axisType == AxisType.X)
        //    manager.BarXLabelHover(point.index);
        //else if (axisType == AxisType.Z)
        //    manager.BarZLabelHover(point.index);
    }

    private void OnTriggerEnter(Collider other)
    {
        SelectionCollider cone = other.gameObject.GetComponent<SelectionCollider>();

        // Collided with controller cone
        if (cone)
        {
            if (axisType == AxisType.X)
                manager.BarXLabelHover(point.index);
            else if (axisType == AxisType.Z)
                manager.BarZLabelHover(point.index);
        }
    }
}
