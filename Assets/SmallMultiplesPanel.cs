using UnityEngine;
using UnityEngine.UI;

public class SmallMultiplesPanel : MonoBehaviour
{
    [SerializeField]
    private Button spawnGraphButton;
    [SerializeField]
    private GameObject graphPrefab;

    private string xVar = "# Time (s)";
    private string yVar = " Z (m)";

    private Vector3 offset = new Vector3(1, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        spawnGraphButton.onClick.AddListener(delegate { spawnGraph(); });
    }

    private void spawnGraph()
    {
        Vector3 position = gameObject.transform.position + offset;
        GameObject smallMultipleGraph = Instantiate(graphPrefab, position, Quaternion.identity);
        SetupChart chart = smallMultipleGraph.GetComponentsInChildren<SetupChart>()[0];
        chart.LoadData(xVar, yVar);
    }
}
