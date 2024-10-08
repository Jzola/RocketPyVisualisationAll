using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BarGraph.VittorCloud
{
    public class BarProperty : MonoBehaviour
    {
        #region publicVariable
        public TextMeshPro BarLabel;
        public GameObject LabelContainer;
        private string barValue;

        public MeshRenderer barMesh;

        public BarMouseClick barClickEvents;
        public float labelScale = 0.06f;

        float ScaleFactor;
        #endregion

        #region privateVariables
        float originalYscale = 0;

        public string BarValue { get => barValue; set => barValue = value; }

        private int barID;
        private Material originalMaterial;
        #endregion

        #region UnityCallBacks


        private void Awake()
        {
            // Debug.Log("SetBarLabelVisible : " + LabelContainer.transform.localScale.y, this.gameObject);
            originalYscale = LabelContainer.transform.localScale.y;
            //Debug.Log("originalYscale : " + LabelContainer.transform.lossyScale.y, this.gameObject);
            LabelContainer.SetActive(false);

        }
      
        public void OnEnable()
        {
            LabelContainer.SetActive(false);
            originalMaterial = barMesh.material;
        }




        #endregion

        #region Customfunctions

        // Creates the label for each bar
        public void SetBarLabelVisible(string value, float scaleFactor)
        {
            // Add text and font
            BarLabel.text = value;
            BarLabel.fontStyle = FontStyles.Normal;

            // Change parent so that we can scale each label to the same dimensions
            // Then set parent object back to original
            LabelContainer.transform.parent = LabelContainer.transform.parent.transform.parent.transform;
            LabelContainer.transform.localScale = new Vector3(labelScale, labelScale, labelScale);
            LabelContainer.transform.parent = this.transform;
            LabelContainer.SetActive(true);
        }

        public void SetBarLabel(string value, float factor)
        {
            BarLabel.text = value;
            LabelContainer.SetActive(false);
            ScaleFactor = factor;


        }

        public void SetLabelEnabel()
        {

            //Debug.Log("SetBarLabelVisible : " + LabelContainer.transform.localScale.y + " : " + transform.localScale. y, this.gameObject);
            if (transform.localScale.y == 0)
                LabelContainer.transform.localScale = new Vector3(LabelContainer.transform.localScale.x, originalYscale / (transform.localScale.x ), LabelContainer.transform.localScale.z);
            else
                LabelContainer.transform.localScale = new Vector3(LabelContainer.transform.localScale.x , originalYscale * ScaleFactor / transform.localScale.y, LabelContainer.transform.localScale.z);

            LabelContainer.SetActive(true);

        }


        public void SetBarColor(Color barColor)
        {

            barMesh.material.color = barColor;
        }

        public Color GetBarColor()
        {

            return barMesh.material.color;
        }


        public void SetBarMat(Material barMat)
        {

            barMesh.material = barMat;
        }

        public void ResetBarMatToOriginal()
        {
            barMesh.material = originalMaterial;
        }

        public void SetBarID(int id)
        {
            barID = id;
        }

        public int GetBarId()
        {
            return barID;
        }
		
        public void BarHover()
        {
            ChartLinkingManager manager = GetComponentInParent<ChartLinkingManager>();
            manager.HighlightID(barID);
        }
        #endregion

    }
}
