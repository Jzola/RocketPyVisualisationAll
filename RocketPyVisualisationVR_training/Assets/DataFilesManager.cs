using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IATK;
using System.IO;
using UnityEngine.UI;

public class DataFilesManager : MonoBehaviour
{
    private string minMaxPath = "/Resources/Scenarios/MinMax/";
    private List<GameObject> dataSources;

    // Start is called before the first frame update
    void Start()
    {
        dataSources = new List<GameObject>();
        loadMinMaxFiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void loadMinMaxFiles()
    {
        string[] filePaths = Directory.GetFiles(Application.dataPath + (minMaxPath), "*.csv");

        for (int i = 0; i < filePaths.Length; i++)
        {
            GameObject dataSourceObj = new GameObject("Scenario" + (i + 1));
            dataSourceObj.transform.SetParent(this.transform, false);
            
            dataSourceObj.AddComponent<CSVDataSource>();
            TextAsset data = TextfromFile(filePaths[i]);

            dataSourceObj.GetComponent<CSVDataSource>().data = data;
            dataSourceObj.GetComponent<CSVDataSource>().loadHeader();
            dataSourceObj.GetComponent<CSVDataSource>().load();

            dataSources.Add(dataSourceObj);
        }
    }

    private TextAsset TextfromFile(string path)
    {
        var sr = new StreamReader(path);
        string contents = sr.ReadToEnd();
        sr.Close();
        return new TextAsset(contents);
    }

    public CSVDataSource GetDataSource(int index)
    {
        return dataSources[index].GetComponent<CSVDataSource>();
    }
}
