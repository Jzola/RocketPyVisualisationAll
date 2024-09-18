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
            camera.fieldOfView = initialFov;
        }
    }

    private void OnGUI()
    {
        float fov = camera.fieldOfView;
        fov -= Input.mouseScrollDelta.y;

        if (fov > maxFov)
            fov = maxFov;

        if (fov < minFov)
            fov = minFov;

        camera.fieldOfView = fov;
    }
}
