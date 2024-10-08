using BarGraph.VittorCloud;
using IATK;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static IATK.DataSource;

public class GraphConfig : GraphCommon
{
    public enum GraphType {SCATTER, BAR};
    private GameObject graph;
    public GameObject VisualisationsParent;
    //public DataFiles datafiles;

    private List<DataFiles> datafilesList;
   
    // Animates the points on update if true, teleports if false.
    public bool tweenPointsOnUpdate = true;
    public bool waitForAllPointsToMove = false;
    private bool variablesInitialised = false;

    private bool graphUpdating = false;
    private bool rocketNeedsReset = false;
    private RocketAnimation rocket;

    [Header("Trajectory Info")]
    [Range(-1, 29)] // Can't adjust at runtime, so using the currently known max
    public int selectedTrajectory = -1; // -1 = All, else an individual trajectory
    private int previousSelection = -1;
    private Filtering filter;

    // Focus data that changes between different inputs
    private CSVDataSource focusData = null;
    public string focusID = "None"; // Datafile / Trajectory ID
    public string focusEngine = "None";
    public string focusType = "None"; // Input variable from the all_inputs.csv file
    public string focusValue = "None"; // Value of the input variable for the specific selection

    // Children of the bar graph selector
    private Transform[] barChildren;
    private bool barPreviouslySpawned = false; // Used to prevent transform from stacking
    private bool barNeedsCreating = false;

    // Start is called before the first frame update
    void Start()
    {    
        Initialise();

        datafilesList = VisualisationsParent.GetComponentsInChildren<DataFiles>().ToList();
        graph = transform.parent.gameObject;
        filter = graph.GetComponentInChildren<Filtering>();
        rocket = graph.GetComponentInChildren<RocketAnimation>();
    }

    // Called every frame
    void Update()
    {
        // If the variables haven't been initialised, attenmpt to do so
        if (!variablesInitialised)
        {
            // Sets the axis variables of the graph, once the graph is made
            List<Visualisation> visualisation = VisualisationsParent.GetComponentsInChildren<Visualisation>().ToList();
            if (visualisation != null)
            {
                setGraphAxisVariables(graph);
                variablesInitialised = true;
            }
        } 
        else if (previousSelection != selectedTrajectory)
        {
            selectTrajectory(selectedTrajectory);
            previousSelection = selectedTrajectory;
        }

        if (!graphUpdating && rocketNeedsReset)
        {
            rocket.gameObject.SetActive(true);
            // Resets rocket animation data
            rocket.setSelectedTrajectory(0);
            rocketNeedsReset = false;
        }
    }

    // Filters out all trajectories except given index, if -1 show all
    public void selectTrajectory(int index)
    {
        filter.ResetFilters();
        if (index >= 0)
        {
            // Sets focus variables
            focusData = filter.FilterSingle(index);
            focusID = focusData.data.name;
            foreach (DataFiles datafiles in datafilesList)
            {
                CSVDataSource inputData = datafiles.input;
                // Finds the engine used for the current trajectory
                focusEngine = inputData.getOriginalString(inputData.findCol("motor type"), inputData["motor type"].Data[index] * (inputData["motor type"].MetaData.categoryCount - 1));

                // Getting the focus value from a given focus type, if a valid match was made in the map. Focus type is the input variable changed.
                if (focusType != "None")
                {
                    try
                    {
                        if (focusType.Contains(","))
                        {
                            string[] focusTypes = focusType.Split(',');
                            focusValue = "";
                            string sep = ", ";

                            // Finds and concatonates the focus values
                            foreach (string type in focusTypes)
                            {
                                DimensionData data = inputData[type];
                                focusValue += (data.MetaData.minValue + (data.Data[index] * (data.MetaData.maxValue - data.MetaData.minValue))).ToString(); // Un-normalise the focus data
                                focusValue += sep;
                            }
                            focusValue = focusValue.Substring(0, focusValue.Length - sep.Length);
                        }
                        else
                        {
                            // Finds and sets the focus value
                            DimensionData data = inputData[focusType];
                            focusValue = (data.MetaData.minValue + (data.Data[index] * (data.MetaData.maxValue - data.MetaData.minValue))).ToString(); // Un-normalise the focus data
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            }
            RocketAnimationUI rocketUI = graph.GetComponentInChildren<RocketAnimationUI>();
            rocketUI.getDropdown().SetValueWithoutNotify(index);
            rocketUI.updateSelectedTrajectory();

        } 
        else
        {
            // Clears focus variables
            focusData = null;
            focusID = "None";
            focusEngine = "None";
            focusValue = "None";
        }

    }

    // Deletes the entire instantiated graph prefab from the scene
    [ContextMenu("Delete Graph")]
    public void DeleteGraph()
    {
        Destroy(graph);
    }

    // Update the graph based on the given settings
    [ContextMenu("Update Graph")]
    public void UpdateGraph()
    {
        selectTrajectory(-1); // Reset selection and filter
        setInputDataset();
        setGraphDimensions(dimensions);

        // Remove axis ticks and labels, since it won't reflect the points while updating.
        foreach (DataFiles datafiles in datafilesList)
            datafiles.removeAxisLabels();

        // Set the axis variables and update trajectories
        setGraphAxisVariables(graph);
        StartCoroutine(updateTrajectories(graph));

        rocketNeedsReset = true;
        rocket.gameObject.SetActive(false);
        //// Resets rocket animation data MOVED TO UPDATE
        //RocketAnimation rocket = graph.GetComponentInChildren<RocketAnimation>();
        //rocket.setSelectedTrajectory(0);
    }

    // Coroutine function for updating trajectories
    private IEnumerator updateTrajectories(GameObject graph)
    {
        graphUpdating = true;

        foreach (DataFiles datafiles in datafilesList)
        {
            // Update progress
            datafiles.trajProgress = 0;
            int index = 0;

            // Finds and updates all the previous points with the new axes
            foreach (Transform dataSet in datafiles.gameObject.transform)
            {
                VisualisationPoints trajectory = dataSet.GetComponentInChildren<VisualisationPoints>();
                if (trajectory != null)
                {
                    trajectory.tweenPointsOnUpdate = tweenPointsOnUpdate;
                    datafiles.UpdateTrajectory(trajectory);

                    // Prevent points from moving during coroutine
                    if (waitForAllPointsToMove) trajectory.pointsNeedUpdating = false;
                }

                // Update progress
                datafiles.trajProgress = (float)index / datafiles.gameObject.transform.childCount; // Progress of trajectory
                index++;

                yield return null;
            }

            datafiles.trajProgress = 1;

            // Enables the points to move once coroutines are over
            if (waitForAllPointsToMove)
            {
                foreach (Transform dataSet in datafiles.gameObject.transform)
                {
                    VisualisationPoints trajectory = dataSet.GetComponentInChildren<VisualisationPoints>();
                    if (trajectory != null)
                    {
                        trajectory.pointsNeedUpdating = trajectory.tweenPointsOnUpdate;
                    }
                }
            }

            datafiles.UpdateAxisTicks();
            datafiles.SetKey();

            if (barNeedsCreating)
            {
                removeThirdLevelBarSelector();
                attachThirdLevelBarSelector();
            }
        }
        graphUpdating = false;
    }

    // Changes the type of the graph
    public void setGraphType(GraphType type)
    {
        // TODO: implement method
        switch (type)
        {
            case GraphType.SCATTER:
                break;
            case GraphType.BAR:
                break;
            default:
                break;
        }
    }

    // Abstracts the given variable on the graph, default is time
    public void setAbstractedVariable(string var)
    {
        // TODO: Change the graphs abstracted variable from time to var
    }

    // Changes the input dataset
    public void setInputDataset()
    {
        foreach (DataFiles datafiles in datafilesList)
        {
            // If the path is unchanged, don't make new simulation data
            if (!datafiles.getSimulationPath().Equals(inputFolderPath + inputFolderName))
            {
                datafiles.DestroyTrajectories();
                //datafiles.setSimulationPath(inputFolderPath, inputFolderName);
                datafiles.setSimulationFilesCoroutine();

                if (isBarGraphAttached()) barNeedsCreating = true;
            }
        }
        focusType = getFocusType();
    }

    // Changes the input value of the given dataset
    public void setInputDatasetValue()
    {
        // TODO: Change the value of the given dataset (e.g. length of rocket, weather condition ...)
    }

    // Resets trajectory filter
    public void resetFilter()
    {
        selectedTrajectory = -1;
        filter.ResetFilters();
    }

    // Attach a bar graph high level selector to the scatter graph
    [ContextMenu("Attach Bar Graph")]
    public void attachThirdLevelBarSelector()
    {
        /*// Prevent extra graphs from being attached
        if (isBarGraphAttached()) return;

        GameObject bargraph = Instantiate(Resources.Load<GameObject>("Prefabs/BarGraph")).GetComponentInChildren<ThirdLevelChartLinkingManager>().gameObject;
        Destroy(bargraph.GetComponentInChildren<DataFiles>().gameObject);
        
        // Add bargraph children to array
        barChildren = new Transform[bargraph.transform.childCount];
        for (int i = 0; i < barChildren.Length; i++)
        {
            barChildren[i] = bargraph.transform.GetChild(i);
        }

        // Change the folder input folder
        bargraph.GetComponentInChildren<LoadInputVariables>().trajectoryFiles = datafiles;
        LoadInputVariables liv = bargraph.GetComponentInChildren<LoadInputVariables>();
        liv.folder = "inputData";
        liv.path = inputFolderPath + inputFolderName + "/";

        // Get input vars to check for input type
        List<string> avaliableInputVariables = new List<string>(File.ReadLines(Application.dataPath + inputFolderPath + inputFolderName + "/inputData/all_inputs.csv").First().Split(','));

        // Set the X axis to the focus variable of the data
        string axisX = focusType == "None" ? liv.axisX : focusType;
        liv.axisX = avaliableInputVariables.Contains(axisX) ? axisX : liv.axisX;

        // Replace blank chart linking manager with the bargraph one
        GameObject chartLinkMgr = graph.GetComponentInChildren<ThirdLevelChartLinkingManager>().gameObject;
        
        // Transform bargraph next to scatter graph
        bargraph.transform.rotation = chartLinkMgr.transform.rotation;
        bargraph.transform.position = chartLinkMgr.transform.position + 
            (barPreviouslySpawned ? new Vector3() : chartLinkMgr.transform.TransformDirection(Vector3.right) * - 4);


        //+new Vector3(-4.56f, -0.75f, -0.47f)
        // Transfer old chartLinkMgr parent/children to new one.
        bargraph.transform.parent = chartLinkMgr.transform.parent;
        for (int i = chartLinkMgr.transform.childCount - 1; i >= 0; i--)
        {
            chartLinkMgr.transform.GetChild(i).SetParent(bargraph.transform);
        }
        
        // Delete old manager
        Destroy(chartLinkMgr);

        barPreviouslySpawned = true;*/
        return;
    }

    [ContextMenu("Remove Bar Graph")]
    // Removes all related gameobjects of the high level bargraph selector
    public void removeThirdLevelBarSelector()
    {
        // Return if it doesn't exist
        if (!isBarGraphAttached()) return;

        graph.GetComponentInChildren<ThirdLevelChartLinkingManager>().enabled = false;
        for (int i = 0; i < barChildren.Length; i++)
        {
            if (barChildren[i] != null)
            {
                DestroyImmediate(barChildren[i].gameObject);
            }
        }
        barChildren = null;
    }

    public bool isBarGraphAttached()
    {
        return graph.GetComponentInChildren<BarGraphGenerator>() != null;
    }

    // Gets the progress of the graph creation/ update, between 0 - 1
    public float getGraphUpdateProgress()
    {
        float progress = 1;
        foreach (DataFiles datafiles in datafilesList)
        {
            if (datafiles.trajProgress < progress)
                progress = datafiles.trajProgress;
        }
        return progress;
    }
}

