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
    private List<GameObject> cameras;
    public SidePanel sidePanel;

    private int currentCamera = 0;
    private int cameraCount;
    void Start()
    {
        cameras = new List<GameObject>();

        Camera[] cameraObjects = camerasParent.GetComponentsInChildren<Camera>();

        foreach (Camera camera in cameraObjects) 
        {
            cameras.Add(camera.gameObject);
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

    public void switchCamera(int cameraId)
    {
        cameras[currentCamera].SetActive(false);
        cameras[cameraId].SetActive(true);
        currentCamera = cameraId;

        sidePanel.SetRocketID(cameraId + 1);
    }
}
