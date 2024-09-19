using System.Collections;
using System.Collections.Generic;
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
        if (currentScenario == questionsList.Count - 1)
            finish();
        else
        {
            currentScenario++;
            currentQuestion = 0;
            currentQuestions = questionsList[currentScenario];
            questionText.text = currentQuestions[currentQuestion];
            cameraController.switchCamera(0);
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
