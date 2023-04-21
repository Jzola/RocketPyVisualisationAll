using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BarGraph.VittorCloud;
using Tilia.Interactions.Interactables.Interactables;
using Tilia.Interactions.Interactables.Interactors;
using System;

public class GraphHandle : MonoBehaviour
{
    GameObject barGraph;
    GameObject barGraphParent;
    InteractableFacade facade;
    Vector3 barGraphOriginalScale;
    bool init;
    // Start is called before the first frame update
    void Start()
    {
        init = false;
        facade = GetComponentInParent<InteractableFacade>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!init)
        {
            ChartLinkingManager test1 = GetComponentInParent<ChartLinkingManager>();
            BarGraphGenerator test2 = test1.GetComponentInChildren<BarGraphGenerator>();
            barGraph = GetComponentInParent<ChartLinkingManager>().GetComponentInChildren<BarGraphGenerator>().gameObject;
            if (barGraph)
            {
                barGraphParent = barGraph.transform.parent.gameObject;
                facade.Ungrabbed.AddListener(UnParent);
                facade.Grabbed.AddListener(ParentBar);
                barGraphOriginalScale = barGraph.transform.localScale;
                init = true;
            }
        }
    }

    private void UnParent(InteractorFacade facade)
    {
        barGraph.transform.parent = barGraphParent.transform;
        barGraph.transform.localScale = barGraphOriginalScale;
    }

    private void ParentBar(InteractorFacade facade)
    {
        barGraph.transform.parent = transform;
        barGraph.transform.localScale = barGraph.transform.localScale * 0.8f;
    }
}
