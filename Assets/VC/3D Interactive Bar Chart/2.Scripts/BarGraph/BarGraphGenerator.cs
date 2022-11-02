using Uitility.VittorCloud;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq.Expressions;
using System.Globalization;
#if UNITY_EDITOR
using UnityEditor;

#endif


namespace BarGraph.VittorCloud
{

    #region CustomEventsDeclaration
    [Serializable]
    public class OnBarPointerDownEvent : UnityEvent<GameObject>
    {
    }
    [Serializable]
    public class OnBarPointerUpEvent : UnityEvent<GameObject>
    {
    }
    [Serializable]
    public class OnBarHoverEnterEvent : UnityEvent<GameObject>
    {

    }
    [Serializable]
    public class OnBarHoverExitEvent : UnityEvent<GameObject>
    {
    }
    #endregion


    // Equivalent of IATK csvDataSource
    [Serializable]
    public class BarGraphDataSet
    {
        public string GroupName = "groupname";
        public Color barColor;
        public Material barMaterial;
        public List<XYBarValues> ListOfBars; // List of data points
        public string axisX = "XaxisName";
        public string axisY = "YaxisName";
        public string axisZ = "ZaxisName";
    }

    // A single data point with x and y value
    [Serializable]
    public class XYBarValues : IComparable<XYBarValues>
    {
        public string XValue; //xvalue
        public float YValue = 0; // yvalue
        public int id; 

        // Used to sort the x-axis values
        public int CompareTo(XYBarValues other)
        {
            try
            {
                // Convert x-axis value to float
                float obj = float.Parse(this.XValue, CultureInfo.InvariantCulture.NumberFormat);
                float otherObj = float.Parse(other.XValue, CultureInfo.InvariantCulture.NumberFormat);

                // Compare objects' x-axis value
                if (obj == otherObj)
                {
                    return 0;
                }
                else if (obj < otherObj)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e);
                return 0;
            }
        }
    }

    public class BarGraphGenerator : MonoBehaviour
    {
        #region publicVariables

        public enum AnimatioType { None = 0, OneByOne = 1, allTogether = 2, animationWithGradient = 3 };
        public enum BarColor { SolidColor = 1, CustumMaterial = 2, HeightWiseGradient = 3 };

        //[Header("Graph Settings")]
        public int MaxHeight = 4;
        public float xStart = 1;
        public float yStart = 1;
        public float zStart = 0;
        public float segmentSizeOnXaxis = 1;
        public float segmentSizeOnYaxis = 1;
        public float segmentSizeOnZaxis = 1;
        public float offsetBetweenXRow = 0;
        public float offsetBetweenZRow = 0;

        [Header("Graph Animation Settings")]
        [Range(0, 15)]
        public float animationSpeed = 1.5f;
        public AnimatioType graphAnimation = AnimatioType.None;

        //[Header("Bar Settings")]
        public GameObject barPrefab;
        public BarColor barColorProperty = BarColor.SolidColor;
        public float barScaleFactor = 1;
        public float xLength = 7.00f; // Size of the x-axis
        public float barWidth = 1.00f; // Width of each individual bar object

        public BarGraphManager GraphRef;

        [HideInInspector]
        public int yMaxValue;   // This sets the max value of y-axis. Need to figure out how to change it.
        [HideInInspector]
        public int yMinValue;

        public Gradient HeightWiseGradient = new Gradient();

        #endregion

        #region privateVariables

        private List<BarGraphDataSet> ListOfDataSet;

        int zMaxSize;
        int xMaxSize;
        int yMaxSize;

        bool zDim = true;
        List<string> xvalues = new List<string>();
        List<float> yvalues = new List<float>();
        List<string> zvalues = new List<string>();
        bool modifyData = false;
        BarGraphManager Graph;
        bool barCompleted = false;
        Color gredientStartColor = Color.white;
        #endregion

        #region CustomEvents

        public OnBarPointerDownEvent OnBarPointerDown;

        public OnBarPointerUpEvent OnBarPointerUp;

        public OnBarHoverEnterEvent OnBarHoverEnter;

        public OnBarHoverExitEvent OnBarHoverExit;

        public UnityEvent OnInitialGraphCompleted;

        #endregion

        #region UnityCallBacks

        public void Start()
        {
            if (OnInitialGraphCompleted == null)
                OnInitialGraphCompleted = new UnityEvent();

            if (OnBarPointerDown == null)
                OnBarPointerDown = new OnBarPointerDownEvent();

            if (OnBarPointerUp == null)
                OnBarPointerUp = new OnBarPointerUpEvent();

            if (OnBarHoverEnter == null)
                OnBarHoverEnter = new OnBarHoverEnterEvent();

            if (OnBarHoverExit == null)
                OnBarHoverExit = new OnBarHoverExitEvent();


            if (graphAnimation == AnimatioType.animationWithGradient)
                barColorProperty = BarColor.HeightWiseGradient;

            if (barColorProperty == BarColor.HeightWiseGradient)
                gredientStartColor = HeightWiseGradient.Evaluate(0);
        }
        private void OnDisable()
        {

            if (Graph != null)
                Graph.GraphUpdated -= OnGraphAnimationCompleted;
        }
        #endregion

        #region GenerateGraphBox
        public void GeneratBarGraph(List<BarGraphDataSet> dataSet)
        {
            if (dataSet == null)
            {
                return;
            }

            ListOfDataSet = dataSet;
            xMaxSize = ListOfDataSet[0].ListOfBars.Count;
            yMaxSize = MaxHeight;
            zMaxSize = ListOfDataSet.Count;

            InitializeGraph();
            AssignGraphAxisName();

            ParseTheDataset();

            if ((int)graphAnimation == 1 || (int)graphAnimation == 3)
                StartCoroutine(CreateBarsWithAnimTypeOneOrThree());
            else
                CreateBarsWithAnimTypeTwo();

            // Set the width of each x-axis label. This could not be done 
            // in the initializeGrapg method because the data had not been
            // set.
            Graph.SetXWidth();

            // Loop each bar group, delete the horizontal layout group object,
            for (int i = 0; i < Graph.ListOfGroups.Count; i++)
            {
                // delete horizontal layout group
                Destroy(Graph.ListOfGroups[i].GetComponent<HorizontalLayoutGroup>());
            }

            // Move the XZ plane to first height of first y-axis node
            //float initialHeight = Graph.GetGraphPoint('y', 0).transform.localPosition.y;
            Graph.XZPlane.transform.localPosition = Graph.XZPlane.transform.localPosition + new Vector3(0f, 0f, 0f);
        }

        private void ParseTheDataset()
        {
            if (ListOfDataSet.Count <= 0)
            {
                Debug.LogError("No data set inserted!!");
                return;
            }
            for (int i = 0; i < ListOfDataSet.Count; i++)
            {

                if (ListOfDataSet[i].ListOfBars.Count <= 0)
                {
                    Debug.LogError("No data set inserted at" + ListOfDataSet[i].GroupName);
                    return;
                }
                for (int j = 0; j < ListOfDataSet[i].ListOfBars.Count; j++)
                {
                    zvalues.Add(ListOfDataSet[i].GroupName);
                    xvalues.Add(ListOfDataSet[i].ListOfBars[j].XValue);
                    yvalues.Add(ListOfDataSet[i].ListOfBars[j].YValue);
                }
            }
            ValidateRange();
            Graph.FetchYPointValues(yMinValue, yMaxValue);
        }


        public void ValidateRange()
        {
            int min = (int)MathExtension.GetMinValue(yvalues);
            int max = (int)MathExtension.GetMaxValue(yvalues);
            int range = max - min;

            // idk the logic behind this. 
            // Terrible at rounding number. It would round 110 to 200 completely ruining a graph.
            //yMinValue = MathExtension.Round(min, range);
            //yMaxValue = MathExtension.Round(max, range);

            if (yMinValue > min)
            {
                yMinValue = yMinValue / 2;
            }

            if (yMaxValue < max)
            {
                yMaxValue = yMaxValue * 2;
            }

            if (yMaxValue <= yMinValue || yMaxValue < max)
            {
                yMaxValue = max;
            }

            // Temporarily fix
            // Set the yMinValue and yMaxValue to -10% and +10% of min
            // This will start the y-axis at an appropriate value rather than 0.
            yMinValue = (int)(min - (min * 0.1));
            yMaxValue = max;// (int)(max + (min * 0.1));

            //if (yMinValue < yMaxValue / 2)
            //{
            //  // yMinValue = 0;
            //}
        }




        void InitializeGraph()
        {
            Graph = Instantiate(GraphRef, transform.position, Quaternion.identity);
            Graph.transform.rotation = this.transform.rotation;
            Graph.transform.parent = this.transform;

            float XLength = xStart + ((xMaxSize - 1) * segmentSizeOnXaxis);
            float YLength = yStart + ((yMaxSize - 1) * segmentSizeOnYaxis);
            float ZLength = zStart + ((zMaxSize - 1) * segmentSizeOnZaxis);
            Graph.setBarScale(barScaleFactor);
            Graph.InitGraphBox(XLength, YLength, ZLength, segmentSizeOnXaxis, segmentSizeOnYaxis, segmentSizeOnZaxis);
            Graph.SetBarRef(barPrefab);
            Graph.InitXAxis(xMaxSize, xStart, segmentSizeOnXaxis, offsetBetweenXRow, xLength);
            Graph.InitYAxis(yMaxSize, yStart, segmentSizeOnYaxis, ListOfDataSet[0].axisY);
            Graph.InitZAxis(zMaxSize, zStart, segmentSizeOnZaxis, offsetBetweenZRow, xStart, segmentSizeOnXaxis);

            Graph.GraphUpdated += OnGraphAnimationCompleted;
        }


        public void AssignGraphAxisName()
        {
            for (int i = 0; i < ListOfDataSet.Count; i++)
            {
                for (int j = 0; j < ListOfDataSet[i].ListOfBars.Count; j++)
                {
                    Graph.AssignAxisName(j, i, ListOfDataSet[i].ListOfBars[j].XValue, ListOfDataSet[i].GroupName, zDim);
                }
            }

            // Set the x/y/z-axis titles
            Graph.xMainLabel.labelText = ListOfDataSet[0].axisX;
            Graph.yMainLabel.labelText = ListOfDataSet[0].axisY;
            Graph.zMainLabel.labelText = ListOfDataSet[0].axisZ;
        }

        public IEnumerator CreateBarsWithAnimTypeOneOrThree()
        {

            for (int i = 0; i < ListOfDataSet.Count; i++)
            {
                for (int j = 0; j < ListOfDataSet[i].ListOfBars.Count; j++)
                {
                    float yscaleFactor = (((yMaxSize - 1) * segmentSizeOnYaxis) + yStart) / (yMaxValue - yMinValue);
                    if (barColorProperty == BarColor.SolidColor)
                        yield return StartCoroutine(Graph.GenerateGraphBarWithAnimTypeOne(j, i, ListOfDataSet[i].ListOfBars[j].YValue, yscaleFactor, animationSpeed, yMinValue, xMaxSize, ListOfDataSet[i].barColor));

                    else if (barColorProperty == BarColor.HeightWiseGradient)
                    {
                        float time = (ListOfDataSet[i].ListOfBars[j].YValue - yMinValue) / (yMaxValue - yMinValue);
                        Color barcolor = HeightWiseGradient.Evaluate(time);

                        if (graphAnimation == AnimatioType.animationWithGradient)
                        {
                            yield return StartCoroutine(Graph.GenerateGraphBarWithAnimTypeThree(j, i, ListOfDataSet[i].ListOfBars[j].YValue, yscaleFactor, animationSpeed, yMinValue, xMaxSize, barcolor, gredientStartColor));
                        }
                        else
                            yield return StartCoroutine(Graph.GenerateGraphBarWithAnimTypeOne(j, i, ListOfDataSet[i].ListOfBars[j].YValue, yscaleFactor, animationSpeed, yMinValue, xMaxSize, barcolor));
                    }
                    else
                        yield return StartCoroutine(Graph.GenerateGraphBarWithAnimTypeOne(j, i, ListOfDataSet[i].ListOfBars[j].YValue, yscaleFactor, animationSpeed, yMinValue, xMaxSize, ListOfDataSet[i].barMaterial));
                    yield return new WaitForSeconds(0.2f);
                }
            }
            Graph.GraphUpdated();
            yield return null;
        }

        public void CreateBarsWithAnimTypeTwo()
        {
            for (int i = 0; i < ListOfDataSet.Count; i++)
            {
                for (int j = 0; j < ListOfDataSet[i].ListOfBars.Count; j++)
                {
                    float yscaleFactor = (((yMaxSize - 1) * segmentSizeOnYaxis) + yStart) / (yMaxValue - yMinValue);
                    if (barColorProperty == BarColor.SolidColor)
                        Graph.GenerateBarWithAnimTypeTwo(j, i, ListOfDataSet[i].ListOfBars[j].YValue, yscaleFactor, animationSpeed, yMinValue, xMaxSize, ListOfDataSet[i].barColor, ListOfDataSet[i].ListOfBars[j].id);
                    else if (barColorProperty == BarColor.HeightWiseGradient)
                    {

                        float time = (ListOfDataSet[i].ListOfBars[j].YValue - yMinValue) / (yMaxValue - yMinValue);
                        Color barcolor = HeightWiseGradient.Evaluate(time);
                        Graph.GenerateBarWithAnimTypeTwo(j, i, ListOfDataSet[i].ListOfBars[j].YValue, yscaleFactor, animationSpeed, yMinValue, xMaxSize, barcolor, ListOfDataSet[i].ListOfBars[j].id);
                    }
                    else
                        Graph.GenerateBarWithAnimTypeTwo(j, i, ListOfDataSet[i].ListOfBars[j].YValue, yscaleFactor, animationSpeed, yMinValue, xMaxSize, ListOfDataSet[i].barMaterial, ListOfDataSet[i].ListOfBars[j].id);
                }
            }
            // If we want the bar creation to animate
            //StartCoroutine(Graph.AnimateBarsWithAnimTypeTwo(animationSpeed));
        }


        // Whether the BarGraph is to be two or three dimensions.
        // This method is needed for generating the main axis titles
        public void ThreeDimensions(bool threeDims)
        {
            zDim = threeDims;
        }

        #endregion

        #region testing Methods

        public void AddNewDataSet(int dataSetIndex, int xyValueIndex, float yValue)
        {
            if (modifyData)
            {
                modifyData = false;
                ListOfDataSet[dataSetIndex].ListOfBars[xyValueIndex].YValue = yValue;
                float yscaleFactor = (yMaxSize * segmentSizeOnYaxis) / (yMaxValue - yMinValue);

                if (barColorProperty == BarColor.HeightWiseGradient)
                {
                    float time = (yValue - yMinValue) / (yMaxValue - yMinValue);
                    Color barcolor = HeightWiseGradient.Evaluate(time);
                    if (graphAnimation == AnimatioType.animationWithGradient)
                    {
                        Graph.UpdateBarHeight(xyValueIndex, dataSetIndex, ListOfDataSet[dataSetIndex].ListOfBars[xyValueIndex].YValue, yscaleFactor, animationSpeed, yMinValue, barcolor, gredientStartColor);
                    }
                    else
                    {
                        Graph.UpdateBarHeight(xyValueIndex, dataSetIndex, ListOfDataSet[dataSetIndex].ListOfBars[xyValueIndex].YValue, yscaleFactor, animationSpeed, yMinValue, barcolor);
                    }
                }

                else
                {
                    Graph.UpdateBarHeight(xyValueIndex, dataSetIndex, ListOfDataSet[dataSetIndex].ListOfBars[xyValueIndex].YValue, yscaleFactor, animationSpeed, yMinValue);
                }
            }
        }


        public void OnGraphAnimationCompleted()
        {
            modifyData = true;
            if (!barCompleted)
            {
                barCompleted = true;
                OnInitialGraphCompleted.Invoke();
            }
        }

        public void ModifyGraph()
        {
            for (int i = 0; i < ListOfDataSet.Count; i++)
            {
                xvalues.RemoveAt(0);
                yvalues.RemoveAt(0);
                int lastIndex = ListOfDataSet[i].ListOfBars.Count - 1;
                xvalues.Add(ListOfDataSet[i].ListOfBars[lastIndex].XValue);
                yvalues.Add(ListOfDataSet[i].ListOfBars[lastIndex].YValue);
                Graph.RemoveAndShiftXpoints(ListOfDataSet[i].ListOfBars[lastIndex].XValue);

                float yscaleFactor = (yMaxSize * segmentSizeOnYaxis) / (yMaxValue - yMinValue);
                if (barColorProperty == BarColor.SolidColor)
                    Graph.GenerateBarWithAnimTypeTwo(lastIndex, i, ListOfDataSet[i].ListOfBars[lastIndex].YValue, yscaleFactor, animationSpeed, yMinValue, xMaxSize, ListOfDataSet[i].barColor, ListOfDataSet[i].ListOfBars[lastIndex].id);
                else
                    Graph.GenerateBarWithAnimTypeTwo(lastIndex, i, ListOfDataSet[i].ListOfBars[lastIndex].YValue, yscaleFactor, animationSpeed, yMinValue, xMaxSize, ListOfDataSet[i].barMaterial, ListOfDataSet[i].ListOfBars[lastIndex].id);
            }
        }
        #endregion
    }
}