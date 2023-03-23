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
                    // If there is a visibility filter component, it will only show visible data fields
                    if (dataVisibility != null)
                    {
                        string[] values = currentDataPoint.GetValuesAsString().TrimEnd('\n').Split('\n');
                        string visibleValues = values[0]; // Will always have the ID added

                        for (int i = 1; i < values.Length; i++)
                        {
                            if (dataVisibility.getVisibility(i-1))
                            {
                                visibleValues += "\n" + values[i];
                            }
                        }
                        display.GetComponentInChildren<Text>(true).text = visibleValues; 
                    } else
                    {
                        display.GetComponentInChildren<Text>(true).text = currentDataPoint.GetValuesAsString();
                    }

                }
            }
        }
    }

    private void InitialLoad()
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

    // Plays/resumes the rocket animation
    [ContextMenu("PlayAnimation")]
    public void playAnimation()
    {
        rocket.setPlaying(true);
    }

    // Pause the rocket animation  
    public void pauseAnimation()
    {
        rocket.setPlaying(false);
    }

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
