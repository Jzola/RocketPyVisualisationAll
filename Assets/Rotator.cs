using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    bool dragging = false;
    Rigidbody rb;
    private Vector3 initialPos;
    private Vector3 initialCameraPos;
    /*[SerializeField]
    private Camera camera;*/
    [SerializeField]
    private float scale = 0.05f;

    private const float maxZoom = 1.551175f;
    private const float minZoom = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPos = transform.position;
        //initialCameraPos = camera.transform.position;
    }

    // Update is called once per frame
    void OnMouseDrag()
    {
        dragging = true;
        Debug.Log(dragging);
    }

    /*private void OnGUI()
    {
        float fov = camera.fieldOfView;
        fov -= Input.mouseScrollDelta.y;

        *//*if (pos.z < maxZoom)
            pos.z = maxZoom;

        if (pos.z > minZoom)
            pos.z = minZoom;*//*

        camera.fieldOfView = fov;
    }*/

    void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            dragging = false;
        }

        if (Input.GetKeyDown("r"))
        {
            reset();
        }
    }

    void FixedUpdate()
    {
        if (dragging)
        {
            float x = Input.GetAxis("Mouse X") * rotationSpeed * Time.fixedDeltaTime;
            float y = Input.GetAxis("Mouse Y") * rotationSpeed * Time.fixedDeltaTime;

            rb.AddTorque(Vector3.down * x);
            rb.AddTorque(Vector3.right * y);
        }
    }

    private void reset()
    {
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.position = initialPos;
        //camera.transform.position = initialCameraPos;
    }
}
