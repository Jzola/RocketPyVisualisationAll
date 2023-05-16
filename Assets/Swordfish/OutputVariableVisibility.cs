using IATK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputVariableVisibility : MonoBehaviour
{
    public GameObject dataFilesObject;
    public bool[] visibilityFilter;

    private List<CSVDataSource> dataSources;
    private int initialNoOfShownVariables = 11; // The amount of variables to have toggled on, starting in order from the first variable columns
    private bool initialized = false;
    public bool filterChanged = false;

    // Start is called before the first frame update
    void Start()
    {
        initiliseData();
    }

    // Runs every frame
    void Update()
    {
        // If class failed to initialise earlier, try again
        if (!initialized)
        {
            initiliseData();
        }
    }

    // Gets the csvdatasources and initialises fields
    private void initiliseData()
    {
        dataSources = dataFilesObject.GetComponent<DataFiles>().GetFiles();
        // If dataSources is empty, don't trigger the initilized flag
        if (dataSources.Count > 0)
        {
            initialiseVisiblities(initialNoOfShownVariables);
            initialized = true;
        }
    }

    // Sets the first given amount of variables to be shown, hiding the rest, resizing the filter beforehand
    public void initialiseVisiblities(int amountVisible)
    {
        visibilityFilter = new bool[dataSources[0].DimensionCount];
        for (int i = 0; i < visibilityFilter.Length; i++)
        {
            visibilityFilter[i] = i < amountVisible ? true : false;
        }
        filterChanged = true;
    }

    // Reset and resize the visibility to show all
    public void resetVisibilty()
    {
        initialiseVisiblities(9999); // Impossible number of variables, to ensure everything is visible
        filterChanged = true;
    }

    // Sets visibilityFilter based on given index
    public void setVisibility(int index, bool visible)
    {
        visibilityFilter[index] = visible;
        filterChanged = true;
    }

    // Sets visibilityFilter based on given variable name
    public void setVisibility(string varName, bool visible)
    {
        int index = dataSources[0].findCol(varName);
        if (index != -1)
        {
            visibilityFilter[index] = visible;
            filterChanged = true;
        }
    }

    // Gets visibility based on index
    public bool getVisibility(int index)
    {
        return visibilityFilter[index];
    }

    // Gets visibility based on variable/ column name
    public bool getVisibility(string varName)
    {
        int index = dataSources[0].findCol(varName);
        if (index != -1)
        {
            return visibilityFilter[index];
        }
        return true;
    }

    // Gets all visibilities
    public bool[] getVisibilities()
    {
        return visibilityFilter;
    }


}
