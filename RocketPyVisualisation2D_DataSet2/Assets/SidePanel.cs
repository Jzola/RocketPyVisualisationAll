using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SidePanel : MonoBehaviour
{
    [SerializeField]
    private Text rocketIDText;
    private int selectedRocket;
    
    private float timeElapsed = 0f;
    private bool active = false;

    private string directory;
    private const string folder = "2D/DataSet2/scenario3";
    private string csvFilePath;

    // Start is called before the first frame update
    void Start()
    {
        directory = Path.Combine(Application.persistentDataPath, folder);
        string fileName = "question_data_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
        csvFilePath = Path.Combine(directory, fileName);

        active = true;
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

    public void SetRocketID(int id)
    {
        selectedRocket = id;

        rocketIDText.text = $"Rocket {id}";
    }

    public void SaveData()
    {
        Directory.CreateDirectory(directory);
        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            writer.WriteLine($"{selectedRocket},{timeElapsed}");
        }
    }
}
