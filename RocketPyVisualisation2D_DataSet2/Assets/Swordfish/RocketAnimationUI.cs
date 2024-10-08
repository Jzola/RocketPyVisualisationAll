using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using IATK;

public class RocketAnimationUI : MonoBehaviour
{
    // UI Dropdown component for selecting which data source to animate the rocket over
    public Dropdown trajectorySourceUIDropdown;
    // UI Slider component for control the point at which the rocket is on within the trajectory
    public Slider trajectoryUISlider;
    // Array of GameObjects containing TextMeshPro component to display the data
    // of the visualisation 
    public GameObject[] dataDisplays;
    public OutputVariableVisibility dataVisibility;

    private bool loaded = false;
    private RocketAnimation rocket;    
    private DataPoint currentDataPoint;
    private DataPoint lastDataPoint;

    void Update()
    {
        if (!loaded)
        {
            InitialLoad();
        } else
        {
            currentDataPoint = rocket.getCurrentDataPoint();
                
            if (currentDataPoint)
            {
                foreach (GameObject display in dataDisplays)
                {
                    // Only updates value string if the filter has changed
                    if (dataVisibility != null && (dataVisibility.filterChanged || currentDataPoint != lastDataPoint))
                    {
                        currentDataPoint.updateValuesString();
                        dataVisibility.filterChanged = false;
                        lastDataPoint = currentDataPoint;
                    }
                    // If there is a visibility filter component, it will only show visible data fields
                    if (dataVisibility != null)
                    {
                        display.GetComponentInChildren<Text>(true).text = currentDataPoint.GetFilteredValuesAsString();
                    } else
                    {
                        display.GetComponentInChildren<Text>(true).text = currentDataPoint.GetRawValuesAsString();
                    }

                }
            }
        }
    }

    public void InitialLoad()
    {
        // Setup dropdown UI with list of data source associated with the visualisation that the rocket is from
        if (trajectorySourceUIDropdown)
        {
            trajectorySourceUIDropdown.ClearOptions();

            rocket = GetComponent<RocketAnimation>();
            if (rocket)
            {
                // Get data files from the DataFiles component of the visualisation
                List<CSVDataSource> dataSourceList = rocket.gameObject.transform.parent.GetComponentInChildren<DataFiles>().GetFiles();

                // DataSourceList has been fully loaded by DataFiles
                if (dataSourceList.Count > 0)
                {
                    // Get the names of those data files as a list
                    List<string> dataSourceNames = new List<string>();
                    foreach (CSVDataSource source in dataSourceList)
                    {
                        dataSourceNames.Add(source.name);
                    }

                    // Add names of data files as dropdown options
                    trajectorySourceUIDropdown.AddOptions(dataSourceNames);
                    loaded = true;
                }                
            }
        }
    }

    [ContextMenu("Play Animation")]
    // Plays/resumes the rocket animation
    public void playAnimation()
    {
        rocket.setPlaying(true);
    }

    [ContextMenu("Pause Animation")]
    // Pause the rocket animation  
    public void pauseAnimation()
    {
        rocket.setPlaying(false);
    }

    [ContextMenu("Reset Animation")]
    // Reset the animation to the beginning
    public void resetAnimation()
    {
        rocket.resetAnimation();
    }

    // Update the trajectory that the rocket is on based on the dropdown value
    public void updateSelectedTrajectory()
    {
        rocket.setSelectedTrajectory(trajectorySourceUIDropdown.value);
    }

    public Slider getPercentSlider()
    {
        return trajectoryUISlider;
    }

    public Dropdown getDropdown()
    {
        return trajectorySourceUIDropdown;
    }
}
