using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zinnia.Action;
using BarGraph.VittorCloud;
using System;
using IATK;

public class SelectionCollider : MonoBehaviour
{
    // Controller action to select a data point to show its data display
    public BooleanAction dataPointSelectionAction;
    // Controller action to select a data point to set the trajectory as the animation source
    public BooleanAction[] trajectoryAnimationSelectionAction;
    // Controller action to select entire trajectory for higlighting with bar chart + slider
    public BooleanAction chartLinkingSelectionAction;

    // dataPointSelectionAction is not being pressed
    private bool dataPointReleased = true;
    private bool chartLinkingReleased = true;

    private void OnTriggerStay(Collider other)
    {
        #region DataPointSelection
        // Activated selectionAction after having released it previously
        if (dataPointReleased && dataPointSelectionAction.IsActivated)
        {
            dataPointReleased = false;

            // Selected object is a DataPoint
            DataPoint point = other.gameObject.GetComponent<DataPoint>();
            if (point)
            {
                // Select DataPoint (i.e. show its data display)
                point.Select();
            }

            Bar bar = other.gameObject.GetComponent<Bar>();
            if (bar)
            {
                bar.SelectBar();
            }
        }

        // Released the DataPoint selection action
        if (dataPointSelectionAction.IsActivated == false)
        {
            dataPointReleased = true;
        }
        #endregion

        #region TrajectoryAnimationSelection
        // Activated the trajectory selection action
        if (trajectoryAnimationSelectionAction[0].IsActivated || trajectoryAnimationSelectionAction[1].IsActivated)
        {
            // Selected object is a DataPoint
            DataPoint point = other.gameObject.GetComponent<DataPoint>();
            if (point)
            {
                // Get the rocket from the visualisation that the selected data point belongs to
                Visualisation visualisation = point.GetComponentInParent(typeof(Visualisation)) as Visualisation;
                RocketAnimation rocket = visualisation.GetComponentInChildren<RocketAnimation>();
                // Visualisation has a rocket
                if (rocket)
                {
                    // Set the rocket to animate over the trajectory of the selected data point
                    rocket.setSelectedTrajectory(point.gameObject);
                }
            }
        }
        #endregion

        // Only works with 2D bar chart
        #region ChartLinkingSelection
        if (chartLinkingReleased && chartLinkingSelectionAction.IsActivated)
        {
            chartLinkingReleased = false;
            DataPoint point = other.gameObject.GetComponent<DataPoint>();
            if (point)
            {
                point.GetComponentInParent<ChartLinkingManager>().LockHighlighting(point.GetTrajectoryID());
            } else
            {
                BarProperty bar = other.gameObject.GetComponentInParent<BarProperty>();
                if (bar)
                {
                    int barID = bar.GetBarId();
                    bar.GetComponentInParent<ChartLinkingManager>().LockHighlighting(barID);
                }
            }
        }
        // Released the DataPoint selection action
        if (chartLinkingSelectionAction.IsActivated == false)
        {
            chartLinkingReleased = true;
        }
        #endregion
    }
}
