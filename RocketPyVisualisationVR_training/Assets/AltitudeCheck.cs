using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltitudeCheck : MonoBehaviour
{
    public float altitude;
    public float graphHeight;
    public float lerpDuration;
    private float timeElapsed;
    private Vector3 initialPos;
    private Vector3 startPoint;
    private Vector3 targetPos;
    private bool moving;

    public void Initialise()
    {
        initialPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (moving) 
        {
            transform.position = Vector3.Lerp(startPoint, targetPos, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;

            if (transform.position == targetPos)
                moving = false;
        }
        
    }

    public void setHeight(float maxHeight, float minHeight)
    {
        Debug.Log("min: " + minHeight);
        Debug.Log("max: " + maxHeight);
        float deltaHeight = maxHeight - minHeight;
        altitude = altitude - minHeight;
        float relativeHeight = altitude / deltaHeight;
        float position = relativeHeight * graphHeight;
        startPoint = transform.position;
        timeElapsed = 0;
        moving = true;

        targetPos = initialPos + new Vector3(0, position, 0);

    }
}
