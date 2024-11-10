using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomCamera : MonoBehaviour
{
    private float initialFov;
    private Camera camera;

    private const float maxFov = 60f;
    private const float minFov = 12f;

    void Start()
    {
        camera = GetComponent<Camera>();
        initialFov = camera.fieldOfView;
    }

    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            ResetZoom();
        }
    }

    private void OnGUI()
    {
        // Change camera fov based on mouse scroll movement
        float fov = camera.fieldOfView;
        fov -= Input.mouseScrollDelta.y;

        // Limit FOV range to prevent excessive zooming in/out
        if (fov > maxFov)
            fov = maxFov;

        if (fov < minFov)
            fov = minFov;

        camera.fieldOfView = fov;
    }

    public void ResetZoom()
    {
        camera.fieldOfView = initialFov;
    }
}
