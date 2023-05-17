using BarGraph.VittorCloud;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Graph.VittorCloud
{
    public class GraphBox : MonoBehaviour
    {
        #region publicVariables

        public GameObject XAxis, YAxis, ZAxis;
        public GraphPoint XPoint, YPoint, ZPoint;
        public GameObject XYPlane, XZPlane, YZPlane;

        public GameObject barParent;
        public float planeSizeOffset = 1;
        public GameObject horizontalGroup;
        public float xLength; // Length of the x-axis
        public GraphPoint yMainLabel;
        public GraphPoint xMainLabel;
        public GraphPoint zMainLabel;

        #endregion

        #region privateVariables
   
        protected List<GraphPoint> ListOfXPoint;
        protected List<GraphPoint> ListOfYPoints;
        protected List<GraphPoint> ListOfZPoints;
        public List<BarGroup> ListOfGroups;
        protected float XpointCount;
        protected float YpointCount;
        protected float ZpointCount;
        protected float graphScaleFactor;

        float XLength;
        float YLength;
        float ZLength;
        float xOffset;
        float yOffset;
        float zOffset;


        #endregion

        #region UnityCallBacks
        // Start is called before the first frame update
        public void Awake()
        {
            ListOfXPoint = new List<GraphPoint>();
            ListOfYPoints = new List<GraphPoint>();
            ListOfZPoints = new List<GraphPoint>();
            ListOfGroups = new List<BarGroup>();

        }
        #endregion

        #region CustomeGraphBehaviours

        public void setBarScale(float scaleFactorvalue)
        {
            graphScaleFactor = scaleFactorvalue;
        }


        public void InitGraphBox(float xLength, float yLength, float zLength, float xSegment, float ySegment, float zSegment)
        {
            XLength = xLength;
            YLength = yLength;
            ZLength = zLength;

           xOffset = xSegment == 0 ? XLength : xSegment;
           yOffset = ySegment == 0 ? YLength : ySegment;
           zOffset = zSegment == 0 ? ZLength : zSegment;

            // Rather than create the planes for each dimension, hide them. This makes the 
            // design somewhat closer to the style of IATK.
            //XYPlane.transform.localScale = new Vector3(XLength + xOffset, YLength + yOffset, XYPlane.transform.localScale.z);
            //YZPlane.transform.localScale = new Vector3(YZPlane.transform.localScale.x, YLength + yOffset, ZLength + zOffset);

            XYPlane.SetActive(false);
            YZPlane.SetActive(false);

            // Set the XZ plane to active 
            // This user will use this plane to compare bar heights
            XZPlane.transform.localScale = new Vector3(XLength + (xOffset*0.5f), XZPlane.transform.localScale.y, ZLength + zOffset);
            XZPlane.SetActive(true);

            barParent.transform.rotation = transform.rotation;
            ////gameObject.AddComponent<BarGraphManager>();
        }

        public void InitXAxis(int xMaxSize, float xStart, float xSegment, float xRowOffset, float xLength)
        {
            this.xLength = xLength;
            // Setting the length of the x-axis object
            XAxis.transform.localScale = new Vector3(xLength, XAxis.transform.localScale.y, XAxis.transform.localScale.z);

            if (xSegment == 0)
            {
                XpointCount = xMaxSize;
            }
            else
            {
                XpointCount = ((XLength - xStart) / xSegment) + 1;
            }

            // Need to calculate the distance between each sphere along the x-axis
            float dis = xLength / XpointCount;

            bool rescaleFontSize = false;
            float sizeDif = 1 - (xLength/XpointCount);
            if (XpointCount > xLength)
            {
                rescaleFontSize = true;
            }

            // Loops number of x-bars and creates x-axis node and label  
            for (int i = 0; i < XpointCount; i++)
            {
                float distance = 1;
                Vector3 pos;

                if (i == 0)
                {
                    distance = xStart;
                    pos = transform.localPosition + new Vector3(dis, 0, 0);
                }
                else
                {
                    distance = xSegment;
                    pos = ListOfXPoint[i - 1].transform.localPosition + new Vector3(dis, 0, 0);
                }

                GraphPoint temp = Instantiate(XPoint, transform.position, transform.rotation);

                temp.transform.parent = transform;
                temp.transform.localPosition = pos;
                temp.transform.localScale = temp.transform.localScale * graphScaleFactor;
                temp.labelContainer.localPosition = new Vector3(0, 0, (-0.75f-ZLength)) / graphScaleFactor;
                // Add box collider to the laebl
                BoxCollider boxCollider = temp.labelContainer.gameObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                // Index of graph point along Z axis
                temp.index = i;

                // If the size of x-axis is smaller than the no. of bars, rescale the 
                // font size of the x-axis labels
                if (rescaleFontSize == true)
                {
                    temp.label.enableAutoSizing = false;
                    temp.label.fontSize -= (temp.label.fontSize * sizeDif);
                }
                ListOfXPoint.Add(temp);
            }

            float xPos = XAxis.transform.localScale.x / 2;

            // Create the x-axis main label
            xMainLabel = Instantiate(ListOfXPoint[0], this.transform, false);
            xMainLabel.transform.localPosition = new Vector3(xPos, -0.5f, 0);
            xMainLabel.label.fontStyle = TMPro.FontStyles.SmallCaps;
            Destroy(xMainLabel.GetComponentInChildren<SphereCollider>().gameObject);
            Destroy(xMainLabel.GetComponentInChildren<BoxCollider>());
        }

        // Set the width of each x-axis label to the number of characters within it's string.
        public void SetXWidth()
        {
            for (int i = 0; i < ListOfXPoint.Count; i++)
            {
                // set the width of the rect transform for the text object
                // to the number of chars in string, thus centering it with XPoint object.
                ListOfXPoint[i].label.rectTransform.sizeDelta = new Vector2(ListOfXPoint[i].labelText.Length+1, 1);
            }
        }

        public void InitYAxis(int yMaxSize, float yStart, float ySegment, string axisName)
        {
            YAxis.transform.localScale = new Vector3(YAxis.transform.localScale.x, YLength + ySegment, YAxis.transform.localScale.z);
            YpointCount = ((YLength - yStart) / ySegment) + 1;

            for (int i = 0; i < YpointCount; i++)
            {
                float distance = 1;
                Vector3 pos;
                if (i == 0)
                {
                    distance = yStart;
                    pos = transform.localPosition + new Vector3(0, distance, 0);
                }
                else
                {
                    distance = ySegment;

                    pos = ListOfYPoints[i - 1].transform.localPosition + new Vector3(0, distance, 0);
                }
                GraphPoint temp = Instantiate(YPoint, transform.localPosition, transform.rotation);
                temp.transform.parent = transform;
                temp.transform.localPosition = pos;

                temp.transform.localScale = temp.transform.localScale * graphScaleFactor;
                temp.labelContainer.localPosition = new Vector3(-0.3f, 0, 0) / graphScaleFactor;
                ListOfYPoints.Add(temp);
            }

            // Create the y-axis main label
            yMainLabel = Instantiate(ListOfYPoints[0], this.transform, false);
            yMainLabel.transform.Rotate(new Vector3(0, 0, 90));
            yMainLabel.transform.localPosition = yMainLabel.transform.localPosition + new Vector3(-1.7f, 0, 0);
            yMainLabel.label.fontStyle = TMPro.FontStyles.SmallCaps;
            Destroy(yMainLabel.GetComponentInChildren<CapsuleCollider>().gameObject);
        }

        public void InitZAxis(int zMaxSize, float zStart, float zSegment, float zRowOffset, float xStart, float xSegment)
        {
            ZAxis.transform.localScale = new Vector3(ZAxis.transform.localScale.x, ZAxis.transform.localScale.y, ZLength + zSegment);

            if (zSegment == 0)
            {
                ZpointCount = zMaxSize;
            }
            else
            {
                ZpointCount = ((ZLength - zStart) / zSegment) + 1;
            }

            for (int i = 0; i < ZpointCount; i++)
            {
                float distance = 1;
                Vector3 pos;
                if (i == 0)
                {
                    distance = zStart;
                    pos = transform.localPosition + new Vector3(0, 0, -distance);
                }
                else
                {
                    distance = zSegment;

                    pos = ListOfZPoints[i - 1].transform.localPosition + new Vector3(0, 0, -distance);
                }
                GraphPoint temp = Instantiate(ZPoint, transform.position, transform.rotation);
                temp.transform.parent = transform;
                temp.transform.localPosition = pos;

                temp.transform.localScale = temp.transform.localScale * graphScaleFactor;
                temp.labelContainer.localPosition = new Vector3(xLength+(xOffset*1.25f), 0, 0);
                // Add box collider to the label 
                BoxCollider boxCollider = temp.labelContainer.gameObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                // Index of graph point along Z axis
                temp.index = i;
                ListOfZPoints.Add(temp);

                GameObject grouptemp = GameObject.Instantiate(horizontalGroup, transform.position, transform.rotation);

                grouptemp.transform.parent = barParent.transform;
                grouptemp.transform.localPosition = pos + new Vector3((zRowOffset * i), 0, 0);

                grouptemp.GetComponent<RectTransform>().sizeDelta = new Vector2(XLength, YLength);
                grouptemp.GetComponent<HorizontalLayoutGroup>().padding.left = (int)xStart;
                grouptemp.GetComponent<HorizontalLayoutGroup>().spacing = (int)xSegment;
                grouptemp.transform.localScale = Vector3.one;
                ListOfGroups.Add(grouptemp.GetComponent<BarGroup>());
            }

            // Create the Z-axis main label
            int approximateCenter = (int)Math.Ceiling((double)ListOfZPoints.Count/2);
            zMainLabel = Instantiate(ListOfZPoints[approximateCenter], this.transform, false);
            zMainLabel.transform.localPosition = zMainLabel.transform.localPosition + new Vector3(2f, 0.5f, 0);
            zMainLabel.label.fontStyle = TMPro.FontStyles.SmallCaps;
            Destroy(zMainLabel.GetComponentInChildren<CapsuleCollider>().gameObject);
        }

        // Sets the main axis title names for x/z-axis
        public void AssignAxisName(int xIndex, int zIndex, string xValue, string zValue, bool zDim)
        {
            if (zDim == true)
            {
                ListOfZPoints[zIndex].labelText = zValue;
            }
            else
            {
                ListOfZPoints[zIndex].labelText = "";
            }
            ListOfXPoint[xIndex].labelText = xValue.ToString();
        }

        // Returns a single GraphPoint object.
        // Parameters specify which axis and index of GraphPoint
        public GraphPoint GetGraphPoint(char axis, int index)
        {
            if (axis == 'x')
            {
                return ListOfXPoint[index];
            }
            else if (axis == 'y')
            {
                return ListOfYPoints[index];
            }
            else if (axis == 'z')
            {
                return ListOfZPoints[index];
            }
            return null;
        }

// TODO will need to change this for consistent spacing between axis label values
        public void FetchYPointValues(int ymin, int ymax)
        {
            float range = ymax - ymin;
            float offset = range / YpointCount;
            float value = ymin;
            for (int i = 0; i < ListOfYPoints.Count; i++)
            {
                value += offset;

                // Currently rounding to 1 decimal place.
                ListOfYPoints[i].labelText = Math.Round((double)value, 1).ToString();
            }
        }

        public void FetchXPointValues(int xmin, int xmax)
        {
            float range = xmax - xmin;
            float offset = range / XpointCount;
            float value = xmin;
            for (int i = 0; i < ListOfXPoint.Count; i++)
            {
                value += offset;
                ListOfXPoint[i].labelText = value.ToString();
            }
        }

        public void FetchZPointValues(int zmin, int zmax)
        {
            float range = zmax - zmin;
            float offset = range / ZpointCount;
            float value = zmin;
            for (int i = 0; i < ListOfZPoints.Count; i++)
            {
                value += offset;
                ListOfZPoints[i].labelText = value.ToString();
            }
        }
        #endregion
    }
}