using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RotationTracker : MonoBehaviour
{
    private List<Rotator> rotators;
    private int activeRotator = 0;
    private float timeElapsed = 0;

    private string directory;
    private const string folder = "2D/DataSet2/tracking";
    private string csvFilePath;
    public bool Running { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        rotators = new List<Rotator>(GetComponentsInChildren<Rotator>());

        directory = Path.Combine(Application.persistentDataPath, folder);
        string fileName = "tracking_data_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
        csvFilePath = Path.Combine(directory, fileName);
    }

    // Update is called once per frame
    void Update()
    {
        if (Running)
            trackRotation();
    }

    private void trackRotation()
    {
        Quaternion rotation = rotators[activeRotator].transform.rotation;
        timeElapsed += Time.deltaTime;
        saveToCSV(rotation);
    }

    private void saveToCSV(Quaternion rotation)
    {
        Directory.CreateDirectory(directory);
        // Write position and rotation data to CSV file
        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            // Format: "GraphNumber,Rotation_X,Rotation_Y,Rotation_Z,Rotation_W, time"
            writer.WriteLine($"{activeRotator},{rotation.x},{rotation.y},{rotation.z},{rotation.w},{timeElapsed}");
        }
    }

    public void SetActiveRotator(int rotator)
    {
        activeRotator = rotator;
    }

    public void ResetRotation()
    {
        foreach (Rotator rotator in rotators)
            rotator.ResetPosition();
    }
}
