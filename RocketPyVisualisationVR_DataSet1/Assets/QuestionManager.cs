using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    [SerializeField]
    private GameObject endPanel;
    [SerializeField]
    private Button submitButton;

    private List<QuestionPanel> questionPanels;
    private List<ToggleGroup> toggleGroups;
    private List<DescriptionPanel> descriptionPanels;
    private List<List<string>> questionsList = new List<List<string>>();
    private List<string> currentQuestions = new List<string>();
    private List<List<string>> descriptionsList = new List<List<string>>();
    private List<string> currentDescriptions = new List<string>();
    private List<List<List<string>>> answersList = new List<List<List<string>>>();
    private List<List<string>> currentAnswers = new List<List<string>>();
    private float timeElapsed = 0;
    private bool running = false;
    private int currentScenario = 0;
    private string csvFilePath;

    // Start is called before the first frame update
    void Start()
    {
        string fileName = "question_data_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
        csvFilePath = Path.Combine(Application.persistentDataPath, fileName);
        questionPanels = new List<QuestionPanel>(GetComponentsInChildren<QuestionPanel>());
        toggleGroups = new List<ToggleGroup>(GetComponentsInChildren<ToggleGroup>(true));
        descriptionPanels = new List<DescriptionPanel>(GetComponentsInChildren<DescriptionPanel>());
        loadQuestions();
        loadDescriptions();
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            timeElapsed += Time.deltaTime;
        }

    }

    public void BeginFirstScenario()
    {
        currentQuestions = questionsList[currentScenario];
        currentDescriptions = descriptionsList[currentScenario];
        setQuestionText();
        setDescriptions();
        foreach (ToggleGroup toggleGroup in toggleGroups)
            toggleGroup.gameObject.SetActive(true);
        running = true;
    }

    private void loadQuestions()
    {
        List<string> scenario1Questions = new List<string>();
        scenario1Questions.Add("Grain Number: What is the general trend observed in the flight paths as the fin span increases?");
        scenario1Questions.Add("Grain Density: What can be inferred about the relationship between grain density and flight time?");
        scenario1Questions.Add("Nose Length: What would likely happen to the trajectory if the nose length were to increase beyond what is shown?");
        scenario1Questions.Add("Fin Distance to CM: Despite varying fin distance to centre mass, what can be concluded about the landing locations of the rockets?");
        scenario1Questions.Add("Fin Span: What can be inferred about the relationship between grain number and flight time?");

        List<string> scenario2Questions = new List<string>();
        scenario2Questions.Add("Grain Number & Wind: In the higher wind speed (60 m/s), how many flight paths make it past 425.2 meters (Y).");
        scenario2Questions.Add("Grain Density & Wind: As the grain density increases, what is the trend in flight path stability in windy conditions?");
        scenario2Questions.Add("Nose Length & Wind: What can be inferred about the relationship between nose length, wind speeds and their effect on the overall height of the flight?");
        scenario2Questions.Add("Fin Distance to CM & Wind: What would likely happen to the trajectory if the Fin Distance to CM were to increase beyond what is shown alongside the wind speed?");
        scenario2Questions.Add("Fin Span & Wind: Despite varying fin spans along with wind speeds, what can be concluded about the landing locations of the rockets?");

        questionsList.Add(scenario1Questions);
        questionsList.Add(scenario2Questions);
    }

    private void loadDescriptions()
    {
        List<string> scenario1Descriptions = new List<string>();
        scenario1Descriptions.Add("Increasing fuel grain number");
        scenario1Descriptions.Add("Increasing fuel grain density");
        scenario1Descriptions.Add("Increasing rocket nose length");
        scenario1Descriptions.Add("Increasing fin distance to centre of mass ");
        scenario1Descriptions.Add("Increasing fin span");

        List<string> scenario2Descriptions = new List<string>();
        scenario2Descriptions.Add("Increasing fuel grain number at varying wind speeds");
        scenario2Descriptions.Add("Increasing fuel grain density at varying wind speeds");
        scenario2Descriptions.Add("Increasing rocket nose length at varying wind speeds");
        scenario2Descriptions.Add("Increasing fin distance to centre of mass at varying wind speeds");
        scenario2Descriptions.Add("Increasing fin span at varying wind speeds");

        descriptionsList.Add(scenario1Descriptions);
        descriptionsList.Add(scenario2Descriptions);
    }

    private void loadAnswers()
    {


        
    }

    private void setQuestionText()
    {
        for (int i = 0; i < questionPanels.Count; i++)
        {
            string text = currentQuestions[i];
            questionPanels[i].SetQuestionText(text);
        }
    }

    private void setDescriptions()
    {
        for (int i = 0; i < descriptionPanels.Count; i++)
        {
            string text = currentDescriptions[i];
            descriptionPanels[i].SetDescription(text);
        }
    }

    public void SubmitAnswers()
    {
        if (checkAnswered())
            NextScenario();
    }

    public void NextScenario()
    {
        saveData();  
        if (currentScenario == questionsList.Count - 1)
            finish();
        else
        {
            currentScenario++;
            currentQuestions = questionsList[currentScenario];
            currentDescriptions = descriptionsList[currentScenario];
            setQuestionText();
            setDescriptions();
            foreach (QuestionPanel panel in questionPanels)
            {
                panel.ResetToggles();
            }
            VisualisationManager visualisationManager = GetComponent<VisualisationManager>();
            visualisationManager.SetActiveScenario(currentScenario);
            timeElapsed = 0;
        }
    }

    private bool checkAnswered()
    {
        bool answered = true;
        foreach (QuestionPanel questionPanel in questionPanels)
        {
            if (questionPanel.Answer == 0)
            {
                Debug.Log(questionPanel.Answer);
                answered = false;
            }
        }

        return answered;
    }

    private void finish()
    {
        endPanel.SetActive(true);
        foreach (QuestionPanel panel in questionPanels)
        {
            panel.GetComponent<CanvasGroup>().interactable = false;
        }
        submitButton.interactable = false;
    }

    private void saveData()
    {
        string answers = "";
        foreach (QuestionPanel panel in questionPanels)
        {
            answers += (panel.Answer + ",");
        }

        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            // Format: "Position_X,Position_Y,Position_Z,Rotation_X,Rotation_Y,Rotation_Z,Rotation_W,Time"
            writer.WriteLine($"{currentScenario},{answers}{timeElapsed}");
        }
    }
}
