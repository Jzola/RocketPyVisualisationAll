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

    private const string folder = "2D/DataSet1/scenario1";

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
        questionText.text = "";
    }

    private void loadQuestions()
    {
        List<string> scenario1Questions = new List<string>();
        scenario1Questions.Add("Mass: What can be inferred about the relationship between mass and flight time?");
        scenario1Questions.Add("Nozzle: What can be inferred about the relationship between nozzle length and flight time?");
        scenario1Questions.Add("Propellant: What is the general trend observed in the flight paths as the propellent length increases?");
        scenario1Questions.Add("Nose to Centre Mass: What would likely happen to the trajectory if the Nose to Centre Mass distance were to increase beyond what is shown?");
        scenario1Questions.Add("Burnout Time: Despite varying burnout times, what can be concluded about the landing locations of the rockets?");

        List<string> scenario2Questions = new List<string>();
        scenario2Questions.Add("Mass & Wind: In the higher wind speed (60 m/s), how many flight paths make it past 425.2 meters (Y).");
        scenario2Questions.Add("Nozzle & Wind: What can be inferred about the relationship between nozzle length, wind speeds and their effect on the overall height of the flight?");
        scenario2Questions.Add("Propellant & Wind: What would likely happen to the trajectory if the propellant were to increase beyond what is shown alongside the wind speed?");
        scenario2Questions.Add("Nose to Centre Mass & Wind: As the distance of the nose to the centre of mass increases, what is the trend in flight path stability in windy conditions?");
        scenario2Questions.Add("Burnout Time & Wind: Despite varying burnout times along with wind speeds, what can be concluded about the landing locations of the rockets?");

        questionsList.Add(scenario1Questions);
        questionsList.Add(scenario2Questions);
    }

    private void loadAnswers()
    {
        string[] question1answers = { "Left to Right", "Right to Left", "Left", "Right" };
        string[] question2answers = { "Unpredictable", "Different Areas", "Same Areas", "Slightly varies" };
        string[] question3answers = { "Shift Right", "Unpredictable", "No Change", "Altitude Decreases" };
        string[] question4answers = { "Left to Right", "Right to Left", "Left", "Right " };
        string[] question5answers = { "No Change ", "Group Up", "Spacing Increases", "Unpredictable" };

        scenario1Answers.Add(question1answers);
        scenario1Answers.Add(question2answers);
        scenario1Answers.Add(question3answers);
        scenario1Answers.Add(question4answers);
        scenario1Answers.Add(question5answers);

        string[] s2question1answers = { "-1246m", "-1300m", "-1370m", "-1454m" };
        string[] s2question2answers = { "6", "4", "3", "2" };
        string[] s2question3answers = { "280m", "333m", "450m", "605m" };
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
        scenario1Descriptions.Add("Increasing grain number");
        scenario1Descriptions.Add("Increasing grain density");
        scenario1Descriptions.Add("Increasing nose length");
        scenario1Descriptions.Add("Increasing fin distance to centre of mass");
        scenario1Descriptions.Add("Increasing fin span");

        List<string> scenario2Descriptions = new List<string>();
        scenario2Descriptions.Add("Increasing grain number at different wind speeds");
        scenario2Descriptions.Add("Increasing grain density at different wind speeds");
        scenario2Descriptions.Add("Increasing nose length at different wind speeds");
        scenario2Descriptions.Add("Increasing fin distance to centre of mass at different wind speeds");
        scenario2Descriptions.Add("Increasing fin span at different wind speeds");

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
        questionText.text = currentQuestions[0];
        times = new float[currentQuestions.Count];
        answers = new int[currentQuestions.Count];
        running = true;
    }

    public void NextScenario()
    {
        saveData();
        cameraController.ResetCameras();
        if (currentScenario == questionsList.Count - 1)
            finish();
        else
        {          
            currentScenario++;
            currentQuestion = 0;
            currentQuestions = questionsList[currentScenario];
            questionText.text = currentQuestions[currentQuestion];         
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

        if (!nextButton.IsActive())
        {
            nextButton.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(false);
        }

        resetToggle();
    }

    private void finish()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = false;
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
}
