using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//For keeping track of the selected rocket
public class SelectionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject completionPanel;

    private float timeElapsed = 0f;
    private bool active = false;
    private int selectedRocketId;

    private string directory;
    private const string folder = "VR/DataSet1/scenario3";
    private string csvFilePath;

    void Start()
    {
        directory = Path.Combine(Application.persistentDataPath, folder);
        string fileName = "question_data_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
        csvFilePath = Path.Combine(directory, fileName);
    }

    void Update()
    {
        if (active)
            timeElapsed += Time.deltaTime;
    }

    public void StartTimer()
    {
        active = true;
    }

    public void SelectRocket(int rocketId)
    {
        selectedRocketId = rocketId;
        saveData();
        completionPanel.SetActive(true);
    }

    private void saveData()
    {
        Directory.CreateDirectory(directory);
        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            writer.WriteLine($"{selectedRocketId},{timeElapsed}");
        }
    }

}
