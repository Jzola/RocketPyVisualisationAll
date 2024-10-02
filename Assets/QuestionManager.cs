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
        loadAnswers();
        loadDescriptions();
        questionText.text = "";
    }

    private void loadQuestions()
    {
        List<string> scenario1Questions = new List<string>();
        scenario1Questions.Add("Identify how many rockets make it past an altitude of 1982 metres");
        scenario1Questions.Add("Identify the direction that the trajectories take along the Y-axis when the nozzle distance increases  ");
        scenario1Questions.Add("Identify how many rockets don't make it past 1631.4 metres along the X axis?");
        scenario1Questions.Add("How does the spacing between trajectories change as the nose length to the centre of mass increases?");
        scenario1Questions.Add("After how many trajectories do you notice irregular spacing / intervals? ");

        List<string> scenario2Questions = new List<string>();
        scenario2Questions.Add("At a windspeed of 20m/s, what is the landing site (Y-axis) at the highest mass rocket?");
        scenario2Questions.Add("At which wind speed do the trajectories show little variance?");
        scenario2Questions.Add("At a windspeed of 40m/s, what is the landing site (X-axis) at the highest rocket propellant distance?");
        scenario2Questions.Add("Which wind speed has the greatest variance in the landing zone?");
        scenario2Questions.Add("How many wind speeds do all trajectories make it past an altitude of 2266 metres?");

        questionsList.Add(scenario1Questions);
        questionsList.Add(scenario2Questions);
    }

    private void loadAnswers()
    {
        string[] question1answers = { "6", "7", "8", "9" };
        string[] question2answers = { "Moves along the positive axis then the negative axis", "Moves along the negative axis then the positive axis ", "Moves along the positive axis", "Moves along the negative axis" };
        string[] question3answers = { "2", "3", "4", "5" };
        string[] question4answers = { "No Changes", "Unpredictable", "Spacing Decreases", "Spacing increases" };
        string[] question5answers = { "4", "5", "6", "7" };

        scenario1Answers.Add(question1answers);
        scenario1Answers.Add(question2answers);
        scenario1Answers.Add(question3answers);
        scenario1Answers.Add(question4answers);
        scenario1Answers.Add(question5answers);

        string[] s2question1answers = { "-1,069.9m to -809.7m", "-809.7m to -549.6m", "-549.6m to -289.4m", "-289.4m to -29.2m" };
        string[] s2question2answers = { "5m/s", "20m/s", "40m/s", "60m/s" };
        string[] s2question3answers = { "915.8m to 1,237.8m", "593.8m to 915.8m", "1,237.8m to 1,559.8m", "1,559.8m to 1,881.8m" };
        string[] s2question4answers = { "5m/s", "20m/s", "40m/s", "60m/s" };
        string[] s2question5answers = { "2", "3", "4", "6" };

        scenario2Answers.Add(s2question1answers);
        scenario2Answers.Add(s2question2answers);
        scenario2Answers.Add(s2question3answers);
        scenario2Answers.Add(s2question4answers);
        scenario2Answers.Add(s2question5answers);
    }

    private void loadDescriptions()
    {
        List<string> scenario1Descriptions = new List<string>();
        scenario1Descriptions.Add("Mass");
        scenario1Descriptions.Add("Nozzle distance to centre of mass");
        scenario1Descriptions.Add("Propellant distance to centre of mass");
        scenario1Descriptions.Add("Nose distance to centre of mass");
        scenario1Descriptions.Add("Fin count");

        List<string> scenario2Descriptions = new List<string>();
        scenario2Descriptions.Add("Mass");
        scenario2Descriptions.Add("Nozzle");
        scenario2Descriptions.Add("Propellant distance to centre of mass");
        scenario2Descriptions.Add("Nose distance to centre of mass");
        scenario2Descriptions.Add("Fin count");

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
