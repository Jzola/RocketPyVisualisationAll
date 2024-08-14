using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltitudeCheck : MonoBehaviour
{
    public float altitude;
    public float graphHeight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setHeight(float maxHeight)
    {
        float relativeHeight = altitude / maxHeight;
        float position = relativeHeight * graphHeight;

        transform.position = transform.position + new Vector3(0, position, 0);

    }
}
