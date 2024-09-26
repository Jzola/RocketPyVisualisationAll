using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RotationTracker : MonoBehaviour
{
    private List<Rotator> rotators;
    private int activeRotator = 0;

    private string directory;
    private const string folder = "2D/DataSet1/tracking";
    private string csvFilePath;

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
        trackRotation();
    }

    private void trackRotation()
    {
        Quaternion rotation = rotators[activeRotator].transform.rotation;
        saveToCSV(rotation);
    }

    private void saveToCSV(Quaternion rotation)
    {
        Directory.CreateDirectory(directory);
        // Write position and rotation data to CSV file
        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            // Format: "GraphNumber,Rotation_X,Rotation_Y,Rotation_Z,Rotation_W"
            writer.WriteLine($"{activeRotator},{rotation.x},{rotation.y},{rotation.z},{rotation.w}");
        }
    }

    public void SetActiveRotator(int rotator)
    {
        activeRotator = rotator;
    }
}
