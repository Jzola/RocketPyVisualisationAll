using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkTransform : MonoBehaviour
{
    public Transform objectToAttach;

    private Vector3 offsetPos;
    private Vector3 offsetUp;
    private Vector3 offsetFwd;

    void Start()
    {
        // Get offsets
        offsetPos = transform.InverseTransformPoint(objectToAttach.position);
        offsetUp = transform.InverseTransformDirection(objectToAttach.up);
        offsetFwd = transform.InverseTransformDirection(objectToAttach.forward);
    }
    void Update()
    {
        // Apply offsets to this transform and set the other objects pos and rotation
        objectToAttach.position = transform.TransformPoint(offsetPos);
        objectToAttach.rotation = Quaternion.LookRotation(transform.TransformDirection(offsetFwd), transform.TransformDirection(offsetUp));
    }
}
