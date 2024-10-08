// using UnityEngine;
// using System.IO;

// public class VRPositionTracker : MonoBehaviour
// {
//     private string csvFilePath;

//     private void Start()
//     {
//         // Construct the file path
//         csvFilePath = Path.Combine(Application.dataPath, "tracking_data.csv");
        
//         // Print out the file path for debugging
//         Debug.Log("CSV File Path: " + csvFilePath);
//     }

//     private void Update()
//     {
//         // Track player position and save to CSV
//         TrackPlayerPosition();
//     }

//     private void TrackPlayerPosition()
//     {
//         // Get player position
//         Vector3 playerPosition = transform.position;

//         // Append position data to CSV
//         SavePositionToCSV(playerPosition);
//     }

//     private void SavePositionToCSV(Vector3 position)
//     {
//         // Write position data to CSV file
//         using (StreamWriter writer = new StreamWriter(csvFilePath, true))
//         {
//             writer.WriteLine(position.x + "," + position.y + "," + position.z);
//         }
//     }
// }


using UnityEngine;
using System.IO;

public class VRPositionTracker : MonoBehaviour
{
    private float timeElapsed = 0f;

    private string directory;
    private const string folder = "VR/DataSet1/tracking";
    private string csvFilePath;

    private void Start()
    {
        // Construct the file path using Application.persistentDataPath
        directory = Path.Combine(Application.persistentDataPath, folder);
        string fileName = "tracking_data_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
        csvFilePath = Path.Combine(directory, fileName);

        // Print out the file path for debugging
        Debug.Log("CSV File Path: " + csvFilePath);
    }

    private void Update()
    {
        // Track player position and save to CSV
        TrackPlayerPosition();
    }

    private void TrackPlayerPosition()
    {
        // Get center eye anchor position and rotation
        Vector3 eyePosition = transform.position;
        Quaternion eyeRotation = transform.rotation;
        timeElapsed += Time.deltaTime;

        // Append position and rotation data to CSV
        SaveDataToCSV(eyePosition, eyeRotation);
    }

    private void SaveDataToCSV(Vector3 position, Quaternion rotation)
    {
        Directory.CreateDirectory(directory);
        // Write position and rotation data to CSV file
        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            // Format: "Position_X,Position_Y,Position_Z,Rotation_X,Rotation_Y,Rotation_Z,Rotation_W,Time"
            writer.WriteLine($"{position.x},{position.y},{position.z},{rotation.x},{rotation.y},{rotation.z},{rotation.w},{timeElapsed}");
        }
    }
}
