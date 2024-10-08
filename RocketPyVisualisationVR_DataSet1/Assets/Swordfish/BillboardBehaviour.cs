using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardBehaviour : MonoBehaviour
{
    void Update()
    {
        Vector3 v = transform.position - GameObject.FindGameObjectWithTag("VRHeadset").transform.position;
        Quaternion q = Quaternion.LookRotation(v);
        transform.rotation = q;
    }
}
