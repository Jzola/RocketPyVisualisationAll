using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    bool dragging = false;
    Rigidbody rb;
    private Vector3 initialPos;
    [SerializeField]
    private float scale = 0.05f;

    private const float maxZoom = 1.551175f;
    private const float minZoom = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPos = transform.position;
    }

    // Update is called once per frame
    void OnMouseDrag()
    {
        dragging = true;
        Debug.Log(dragging);
    }

    void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            dragging = false;
        }

        if (Input.GetKeyDown("r"))
        {
            ResetPosition();
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

    public void ResetPosition()
    {
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.position = initialPos;
    }
}
