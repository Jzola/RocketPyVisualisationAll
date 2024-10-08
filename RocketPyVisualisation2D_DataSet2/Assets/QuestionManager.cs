using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    [SerializeField]
    private Text questionText;
    [SerializeField]
    private GameObject answerToggles;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private cameracacameraScript cameraController;
    [SerializeField]
    private Text descriptionText;
    [SerializeField]
    private GameObject legend;

    private List<Toggle> toggles;
    private ToggleGroup toggleGroup;
    private List<Text> toggleText;

    private int currentQuestion = 0;
    private int answer;
    private int currentScenario = 0;
    private int[] answers;
    private float[] times;
    private bool running;

    private List<List<string>> questionsList = new List<List<string>>();
    private List<string> currentQuestions = new List<string>();

    private List<string[]> scenario1Answers = new List<string[]>();
    private List<string[]> scenario2Answers = new List<string[]>();
    private List<string[]> currentAnswers = new List<string[]>();

    private List<List<string>> descriptionsList = new List<List<string>>();
    private List<string> currentDescriptions = new List<string>();

    private string csvFilePath;
    private string directory;

    private const string folder = "2D/DataSet2/scenario1";

    // Start is called before the first frame update
    void Start()
    {
        toggles = new List<Toggle>(answerToggles.GetComponentsInChildren<Toggle>(true));
        toggleGroup = answerToggles.GetComponent<ToggleGroup>();
        toggleText = new List<Text>(answerToggles.GetComponentsInChildren<Text>(true));

        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener(delegate
            {
                checkToggle(toggle);
            });
        }

        directory = Path.Combine(Application.persistentDataPath, folder);
        string fileName = "question_data_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".csv";
        csvFilePath = Path.Combine(directory, fileName);

        loadQuestions();
        loadAnswers();
        loadDescriptions();
        questionText.text = "";
    }

    private void loadQuestions()
    {
        List<string> scenario1Questions = new List<string>();
        scenario1Questions.Add("As the variable increases, what is the general trend of the graph along the Y-axis? ");
        scenario1Questions.Add("How does increasing the variable affect the landing site of the rocket?");
        scenario1Questions.Add("If we were to increase the number of trajectories beyond what is displayed, what would be the likely outcome?");
        scenario1Questions.Add("Identify the direction that the flight paths take along the X-axis as the variable increases");
        scenario1Questions.Add("As the variable increases, what is the general trend of the trajectories?");

        List<string> scenario2Questions = new List<string>();
        scenario2Questions.Add("At a windspeed of 60m/s, what is the landing site (Y-axis) at the largest variable?");
        scenario2Questions.Add("At how many wind speeds do all trajectories make it past an altitude of 1415m? ");
        scenario2Questions.Add("At a windspeed of 40m/s, what is the landing site (X-axis) for the largest variable ");
        scenario2Questions.Add("At how many wind speeds do all trajectories make it past a landing site (Y-axis) of -1662 metres? ");
        scenario2Questions.Add("Which wind speed exhibits the greatest difference in altitude across all trajectories");

        questionsList.Add(scenario1Questions);
        questionsList.Add(scenario2Questions);
    }

    private void loadAnswers()
    {
        string[] question1answers = { "Moves in the negative direction then the positive direction ", "Moves in the positive direction then the negative direction ", "Moves in the positive direction", "Moves in the negative direction" };
        string[] question2answers = { "Different areas", "No change", "Slightly varies", "Unpredictable" };
        string[] question3answers = { "Altitude increases", "Altitude decreases", "No change", "Unpredictable behaviour" };
        string[] question4answers = { "Moves in the negative direction then the positive direction ", "Moves in the positive direction then the negative direction ", "Moves in the positive direction", "Moves in the negative direction" };
        string[] question5answers = { "Spacing Increases", "Spacing decreases", "Spacing increases", "Unpredictable" };

        scenario1Answers.Add(question1answers);
        scenario1Answers.Add(question2answers);
        scenario1Answers.Add(question3answers);
        scenario1Answers.Add(question4answers);
        scenario1Answers.Add(question5answers);

        string[] s2question1answers = { "–2,077.6m to -1,869.8m", "-1869.8m to -1,662.0m", "-1,662.0m to -1,454.2m", "-1454m to -1,246.4m " };
        string[] s2question2answers = { "6", "4", "3", "2" };
        string[] s2question3answers = { "61.2m to 333.3m", "333.3m to 605.5m", "605.5m to 877.7m", "877.7m to 1,149.8m" };
        string[] s2question4answers = { "1", "2", "3", "4" };
        string[] s2question5answers = { "60m/s", "40m/s", "20m/s", "5m/s" };

        scenario2Answers.Add(s2question1answers);
        scenario2Answers.Add(s2question2answers);
        scenario2Answers.Add(s2question3answers);
        scenario2Answers.Add(s2question4answers);
        scenario2Answers.Add(s2question5answers);
    }

    private void loadDescriptions()
    {
        List<string> scenario1Descriptions = new List<string>();
        scenario1Descriptions.Add("Grain number");
        scenario1Descriptions.Add("Grain density");
        scenario1Descriptions.Add("Nose length");
        scenario1Descriptions.Add("Fin distance to centre of mass");
        scenario1Descriptions.Add("Fin span");

        List<string> scenario2Descriptions = new List<string>();
        scenario2Descriptions.Add("Grain number");
        scenario2Descriptions.Add("Grain density");
        scenario2Descriptions.Add("Nose length");
        scenario2Descriptions.Add("Fin distance to centre of mass");
        scenario2Descriptions.Add("Fin span");

        descriptionsList.Add(scenario1Descriptions);
        descriptionsList.Add(scenario2Descriptions);
    }

    void Update()
    {
        if (running)
            times[currentQuestion] += Time.deltaTime;

        if (!toggleGroup.AnyTogglesOn())
            nextButton.interactable = false;
        else if (!nextButton.IsInteractable())
            nextButton.interactable = true;
    }

    public void BeginFirstScenario()
    {
        currentQuestions = questionsList[currentScenario];
        currentAnswers = scenario1Answers;
        currentDescriptions = descriptionsList[0];

        questionText.text = currentQuestions[0];
        setAnswerText();
        descriptionText.text = currentDescriptions[currentQuestion];

        times = new float[currentQuestions.Count];
        answers = new int[currentQuestions.Count];
        running = true;
    }

    public void NextScenario()
    {
        saveData();
        cameraController.ResetCameras();
        if (currentScenario == questionsList.Count - 1)
        {
            legend.SetActive(false);
            descriptionText.transform.parent.gameObject.SetActive(false);
            finish();
        }
        else
        {
            currentScenario++;
            currentQuestion = 0;
            currentQuestions = questionsList[currentScenario];
            currentAnswers = scenario2Answers;
            currentDescriptions = descriptionsList[currentScenario];

            if (currentScenario == 1)
            {
                legend.SetActive(true);
            }

            questionText.text = currentQuestions[currentQuestion];
            setAnswerText();
            descriptionText.text = currentDescriptions[currentQuestion];

            answers = new int[currentQuestions.Count];
            times = new float[currentQuestions.Count];
            resetToggle();
        }
    }

    public void NextQuestion()
    {
        Debug.Log("Question " + (currentQuestion + 1) + " Answer: " + answers[currentQuestion] + " Time: " + times[currentQuestion]);

        if (answers[currentQuestion] == 0)
            return;

        currentQuestion++;
        questionText.text = currentQuestions[currentQuestion];
        setAnswerText();
        descriptionText.text = currentDescriptions[currentQuestion];

        if (currentQuestion == currentQuestions.Count - 1)
        {
            nextButton.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(true);
        }
        resetToggle();      
    }

    public void PrevQuestion()
    {
        if (currentQuestion == 0)
            return;
        currentQuestion--;
        questionText.text = currentQuestions[currentQuestion];
        setAnswerText();
        descriptionText.text = currentDescriptions[currentQuestion];

        if (!nextButton.IsActive())
        {
            nextButton.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(false);
        }

        resetToggle();
    }

    private void finish()
    {
        
    }

    private void saveData()
    {
        Directory.CreateDirectory(directory);
        using (StreamWriter writer = new StreamWriter(csvFilePath, true))
        {
            for (int i = 0; i < times.Length; i++)
            {
                // Format: "Answer,Time"
                writer.WriteLine($"{answers[i]},{times[i]}");
                Debug.Log("Question " + (i + 1) + "\nAnswer: " + answers[i] + "  Time: " + times[i]);
            }
        }
    }

    private void checkToggle(Toggle change)
    {
        int selected = 0;
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i].isOn)
                selected = i + 1;
        }
        answers[currentQuestion] = selected;
        Debug.Log($"Question {currentQuestion + 1} Answer: {answers[currentQuestion]}");
    }

    private void resetToggle()
    {     
        for (int i = 0; i < toggles.Count; i++)
        {
            if (i + 1 == answers[currentQuestion])
                toggles[i].SetIsOnWithoutNotify(true);
            else
                toggles[i].SetIsOnWithoutNotify(false);
        }
    }

    private void setAnswerText()
    {
        for (int i = 0; i < toggleText.Count; i++)
        {
            toggleText[i].text = currentAnswers[currentQuestion][i];
        }
    }
}
