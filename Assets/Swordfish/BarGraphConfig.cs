using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;

public class BarGraphConfig : MonoBehaviour
{
    private GameObject graph;
    // Start is called before the first frame update
    void Start()
    {
        graph = transform.parent.gameObject;
    }

    [ContextMenu("DeleteGraph")] 
    public void deleteGraph()
    {
        Destroy(transform.parent.gameObject);
    }
}
