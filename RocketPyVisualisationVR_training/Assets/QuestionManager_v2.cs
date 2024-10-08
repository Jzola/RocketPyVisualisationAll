using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class QuestionManager_v2 : MonoBehaviour
{
    [SerializeField]
    private Text questionText;
    [SerializeField]
    private GameObject answerToggles;
    [SerializeField]
    private Text descriptionText;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private GraphSetManager graphManager;
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

    private string directory;
    private const string folder = "VR/Training/scenario1";
    private string csvFilePath;

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
        scenario1Questions.Add("As the variable increases, what is the general trend of the trajectories");

        List<string> scenario2Questions = new List<string>();
        scenario2Questions.Add("At a windspeed of 25m/s, what is the peak altitude of the smallest variable");

        questionsList.Add(scenario1Questions);
        questionsList.Add(scenario2Questions);
    }

    private void loadAnswers()
    {
        string[] question1answers = { "Spacing increases", "Spacing decreases", "No changes", "Unpredictable" };

        scenario1Answers.Add(question1answers);

        string[] s2question1answers = { "805.8m to 908.3m", "704.9m to 805.8m", "604.m to 704.9m", "503.5m to 604.4m" };

        scenario2Answers.Add(s2question1answers);
    }

    private void loadDescriptions()
    {
        List<string> scenario1Descriptions = new List<string>();
        scenario1Descriptions.Add("Fin distance to centre of mass");

        List<string> scenario2Descriptions = new List<string>();
        scenario2Descriptions.Add("Fin distance to centre of mass");

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
        descriptionText.text = currentDescriptions[0];

        times = new float[currentQuestions.Count];
        answers = new int[currentQuestions.Count];
        running = true;
    }

    public void NextScenario()
    {
        saveData();
        if (currentScenario == questionsList.Count - 1)
        {
            legend.SetActive(false);
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
        if (answers[currentQuestion] == 0)
            return;

        currentQuestion++;
        questionText.text = currentQuestions[currentQuestion];
        setAnswerText();
        descriptionText.text = currentDescriptions[currentQuestion];

        if (currentQuestion == currentQuestions.Count - 1)
        {
            submitButton.gameObject.SetActive(true);
        }
        resetToggle();
    }

    private void finish()
    {
        //graphManager.SpawnMultiGraphSet();
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
