using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateParent : MonoBehaviour
{
    float rotSpeed = 120;

    void OnMouseDrag()
    {
        float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

        transform.parent.transform.Rotate(Vector3.up, -rotX);
       //  transform.parent.transform.Rotate(Vector3.right, rotY);
    }
}
