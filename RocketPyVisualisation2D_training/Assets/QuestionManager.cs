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

    private const string folder = "2D/Training/scenario1";

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
        scenario1Questions.Add("Identify the direction that the trajectories take along the Y-axis when the variable increases.");

        List<string> scenario2Questions = new List<string>();
        scenario2Questions.Add("At a windspeed of 10m/s, what is the landing site (X-axis) at the smallest variable");

        questionsList.Add(scenario1Questions);
        questionsList.Add(scenario2Questions);
    }

    private void loadAnswers()
    {
        string[] question1answers = { "Moves in the positive direction", "Moves in the negative direction", "Moves in the positive direction then the negative direction", "Moves in the negative direction then the positive direction" };

        scenario1Answers.Add(question1answers);

        string[] s2question1answers = { "-679.8m to –503.9m", "-503.9m to –328.1m", "-328.1m to –152.2m", "-152.2m to 23.7m" };

        scenario2Answers.Add(s2question1answers);

    }

    private void loadDescriptions()
    {
        List<string> scenario1Descriptions = new List<string>();
        scenario1Descriptions.Add("Rocket nozzle distance");

        List<string> scenario2Descriptions = new List<string>();
        scenario2Descriptions.Add("Rocket nozzle distance");

        descriptionsList.Add(scenario1Descriptions);
        descriptionsList.Add(scenario2Descriptions);
    }

    void Update()
    {
        if (running)
            times[currentQuestion] += Time.deltaTime;
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
        if (currentScenario == questionsList.Count)
        {

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

        resetToggle();
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
