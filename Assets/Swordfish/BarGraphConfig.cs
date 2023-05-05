using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarGraphConfig : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [ContextMenu("DeleteGraph")] 
    public void deleteGraph()
    {
        Destroy(transform.parent.gameObject);
    }
}
