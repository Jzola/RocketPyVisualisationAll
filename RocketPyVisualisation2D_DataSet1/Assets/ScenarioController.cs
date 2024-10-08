using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioController : MonoBehaviour
{
    private List<string> scenarios;
    [HideInInspector]
    public string CurrentScenario;

    public event EventHandler ScenarioChanged;

    // Start is called before the first frame update
    void Start()
    {
        scenarios = new List<string>();
        //TODO: Load scenario names
        //for now use hard coded names
        scenarios.Add("Scenario1");
        scenarios.Add("Scenario2");

        //Default to first scenario
        CurrentScenario = scenarios[0];
    }

    public void ChangeScenario(int scenario)
    {
        CurrentScenario = scenarios[scenario];
        ScenarioChanged?.Invoke(this, EventArgs.Empty);
    }
}
