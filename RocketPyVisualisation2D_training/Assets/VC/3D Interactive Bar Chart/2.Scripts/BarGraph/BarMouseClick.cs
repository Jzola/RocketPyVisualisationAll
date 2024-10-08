using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IATK;

namespace BarGraph.VittorCloud
{
    public class BarMouseClick : MonoBehaviour
    {
        #region PublicVariables

        public Vector3 barScale;
        public Outline outline;

        public Action<GameObject> PointerDownOnBar;
        public Action<GameObject> PointerUpOnBar;
        public Action<GameObject> PointerEnterOnBar;
        public Action<GameObject> PointerExitOnBar;

        private ChartLinkingManager manager;
        private int barID;
        #endregion

        #region PrivateVariables

        GameObject bar;
        #endregion

        #region UnityCallBacks

        private void Awake()
        {
            bar = transform.parent.gameObject;
        }
        // Start is called before the first frame update
        void Start()
        {

            barScale = transform.localScale;
            outline.enabled = false;

            manager = GetComponentInParent<ChartLinkingManager>();
            barID = GetComponentInParent<BarProperty>().GetBarId();
        }



        #region UnityMouseEvents
        public void OnMouseDown()
        {
            transform.localScale = transform.localScale + new Vector3(0.15f, 0, 0.15f);
            outline.enabled = true;
            PointerDownOnBar(bar);

            manager.LockHighlighting(barID);
            GetComponent<Bar>().SelectBar();
        }
        public void OnMouseUp()
        {
            transform.localScale = barScale;
            outline.enabled = false;
            PointerUpOnBar(bar);
        }
        public void OnMouseEnter()
        {
            transform.localScale = transform.localScale + new Vector3(0.15f, 0, 0.15f);
            PointerEnterOnBar(bar);
            //outline.enabled = true;
        }
        public void OnMouseExit()
        {
            transform.localScale = barScale;
            outline.enabled = false;
            PointerExitOnBar(bar);
        }
        #endregion

        #endregion
    }
}
