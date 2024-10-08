using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnDistance : MonoBehaviour
{
    // Object to calc distance from
    public GameObject otherObject;

    // Alternative way to get distance. Used since Destination gives fake pos
    public GameObject objectZLength;

    // Control the scaling algorithm
    public float maxScale = 1;
    public float minScale = 0;
    public float maxDistance = 3;
    public float distance;

    // Update is called once per frame
    void Update()
    {
        // If an object is provided for z length, use that number instead. Otherwise, get distance between this object and the other object.
        distance = objectZLength != null ? objectZLength.transform.localScale.z : Vector3.Distance(transform.position, otherObject.transform.position);
        
        // Calculate scale using set variables.
        float newScale = Mathf.Clamp(distance / maxDistance, 0, 1) * (maxScale - minScale) + minScale;

        // Set scale of this object.
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}
