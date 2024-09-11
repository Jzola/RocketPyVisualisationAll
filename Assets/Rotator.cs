using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    bool dragging = false;
    Rigidbody rb;
    private Vector3 initialPos;
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
    }
}
