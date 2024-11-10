using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cameracacameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject camera0;
    public GameObject camera1;
    public GameObject camerasParent;
    private List<Camera> cameras;
    public SidePanel sidePanel;

    [SerializeField]
    private RotationTracker tracker;

    private int currentCamera = 0;
    private int cameraCount;

    public const int cameraFov = 60;

    void Start()
    {
        // All cameras active at startup and added to list
        cameras = new List<Camera>(camerasParent.GetComponentsInChildren<Camera>());

        // Deactivate each camera
        foreach (Camera camera in cameras) 
        {
            camera.gameObject.SetActive(false);
        }

        // Switch to the first camera
        switchCamera(0);
    }

    public void nextCamera()
    {
        cameraCount++;
        // If moving past the last camera, move back to the first camera
        if (cameraCount > cameras.Count - 1)
        {
            cameraCount = 0;
        }

        switchCamera(cameraCount);
    }

    public void prevCamera()
    {
        cameraCount--;
        if (cameraCount < 0)
        {
            // If moving before the first camera, move to the last camera
            cameraCount = cameras.Count - 1;
        }

        switchCamera(cameraCount);
    }

    public void ResetCameras()
    {
        // Reset to first camera and reset the zoom of all cameras
        cameraCount = 0;
        switchCamera(0);
        foreach (Camera camera in cameras)
            camera.fieldOfView = cameraFov;
    }

    public void switchCamera(int cameraId)
    {
        // Disable the currently active camera and enable the specified camera
        cameras[currentCamera].gameObject.SetActive(false);
        cameras[cameraId].gameObject.SetActive(true);
        currentCamera = cameraId;

        // Update the ui and metrics tracker with the newly active camera
        sidePanel.SetRocketID(cameraId + 1);
        tracker.SetActiveRotator(cameraId);
    }
}
