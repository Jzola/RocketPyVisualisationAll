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
        cameras = new List<Camera>(camerasParent.GetComponentsInChildren<Camera>());

        foreach (Camera camera in cameras) 
        {
            camera.gameObject.SetActive(false);
        }

        switchCamera(0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void nextCamera()
    {
        cameraCount++;
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
            cameraCount = cameras.Count - 1;
        }

        switchCamera(cameraCount);
    }

    public void ResetCameras()
    {
        cameraCount = 0;
        switchCamera(0);
        foreach (Camera camera in cameras)
            camera.fieldOfView = cameraFov;
    }

    public void switchCamera(int cameraId)
    {
        cameras[currentCamera].gameObject.SetActive(false);
        cameras[cameraId].gameObject.SetActive(true);
        currentCamera = cameraId;

        sidePanel.SetRocketID(cameraId + 1);
        tracker.SetActiveRotator(cameraId);
    }
}
