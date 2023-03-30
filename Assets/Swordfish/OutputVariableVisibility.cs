using IATK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputVariableVisibility : MonoBehaviour
{
    public GameObject dataFilesObject;
    private List<CSVDataSource> dataFiles;
    public bool[] visibilityFilter;

    private int initialNoOfShownVariables = 11;

    // Start is called before the first frame update
    void Start()
    {
        dataFiles = dataFilesObject.GetComponent<DataFiles>().GetFiles();
        initialiseVisiblities(initialNoOfShownVariables);
    }

    // Sets the first given amount of variables to be shown, hiding the rest, and resizes the filter
    public void initialiseVisiblities(int amountVisible)
    {
        visibilityFilter = new bool[dataFiles[0].DimensionCount];
        for (int i = 0; i < visibilityFilter.Length; i++)
        {
            visibilityFilter[i] = i < amountVisible ? true : false;
        }
    }

    // Reset and resize the visibility to show all
    public void resetVisibilty()
    {
        initialiseVisiblities(9999);
    }

    // Sets visibilityFilter based on given index
    public void setVisibility(int index, bool visible)
    {
        visibilityFilter[index] = visible;
    }

    // Sets visibilityFilter based on given variable name
    public void setVisibility(string varName, bool visible)
    {
        int index = dataFiles[0].findCol(varName);
        if (index != -1)
        {
            visibilityFilter[index] = visible;
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
        int index = dataFiles[0].findCol(varName);
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
