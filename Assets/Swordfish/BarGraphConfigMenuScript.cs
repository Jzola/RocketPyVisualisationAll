using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarGraphConfigMenuScript : MonoBehaviour
{
    private BarGraphConfig gConfig;
    private bool barSelectorAttached = false;
    // Start is called before the first frame update
    void Start()
    {
        //delete button is already handled separately.
        gConfig = transform.parent.gameObject.GetComponent<BarGraphConfig>();
        
        //add listener to button to call
        

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnLinkedBarGraph()
    {
        if (!barSelectorAttached)
        {
            //gConfig.attachthirdlevelbarselector()

        }
        else
        {
            //
        }
    }


}
