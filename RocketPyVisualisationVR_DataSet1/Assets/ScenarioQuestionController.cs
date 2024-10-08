using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioQuestionController : MonoBehaviour
{
    [SerializeField]
    private Text scenarioTitle;
    [SerializeField]
    private Text questionTitle;
    [SerializeField]
    private Text questionText;
    [SerializeField]
    private GameObject answerToggles;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private Button prevButton;

    private List<Toggle> toggles;
    private List<Text> toggleText;

    private List<List<string>> questionsList = new List<List<string>>();
    private List<string> currentScenarioQuestions;
    private int currentQuestion = 0;
    private int currentScenario = 0;
    private int answer;
    private int[] answers = { 0, 0, 0, 0, 0, 0, 0 };
    private float elapsedTime = 0;
    private bool running;
    private float[] times;

    // Start is called before the first frame update
    void Start()
    {
        loadQuestions();
        running = true;

        toggles = new List<Toggle>(answerToggles.GetComponentsInChildren<Toggle>());
        toggleText = new List<Text>(answerToggles.GetComponentsInChildren<Text>());

        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener(delegate
            {
                checkToggle(toggle);
            });
        }

        scenarioTitle.text = "Scenario " + (currentScenario + 1) + " Questions";
        currentScenarioQuestions = questionsList[currentScenario];
        times = new float[currentScenarioQuestions.Count];
        setQuestionText();
    }

    void Update()
    {
        if (running)
        {
            times[currentQuestion] += Time.deltaTime;
        }
    }

    //Hard coding question text for now
    private void loadQuestions()
    {
        List<string> scenario1Questions = new List<string>();
        scenario1Questions.Add("Mass: How does increasing the mass affect the altitude and duration of the rocket's flight path?");
        scenario1Questions.Add("Top Radius: How does changing the top radius impact the trajectory?");
        scenario1Questions.Add("Tail Length: How does adjusting the length of the tail affect the rocket’s flight duration and stability? ");
        scenario1Questions.Add("Nose to CM: How does increasing the distance from the nose to the centre of mass affect the performance of the rocket’s flight path? ");
        scenario1Questions.Add("Tail to CM: How does altering the distance from the tail to the centre of mass affect the performance of the rocket’s flight path? ");
        scenario1Questions.Add("Identify which individual Rocket Value has the greatest effect on the flight path of the rocket");
        scenario1Questions.Add("Identify which individual Rocket Value has the least effect on the flight path of the rocket ");

        List<string> scenario2Questions = new List<string>();
        scenario2Questions.Add("Mass & Tail Length: What is the effect of altering the rocket’s mass in combination with the tail length to affect the flight duration, stability, and trajectory? ");
        scenario2Questions.Add("Mass & Nose to CM: How does varying the rocket’s mass in combination with the distance between the nose and the centre of mass affect the stability and trajectory of the rocket? ");
        scenario2Questions.Add("Mass & Tail to CM: What is the combined effect of changing both the rocket mass and the distance between the tail to the centre of mass, affect the rocket’s flight path? ");
        scenario2Questions.Add("Mass & Nozzle: How does variation in the rocket’s mass combined with differing nozzle designs affect the flight path of the rocket? ");
        scenario2Questions.Add("Mass & Propellant: What is the impact of changing both rocket mass and the distance of the propellant, have on the overall flight performance and stability? ");
        scenario2Questions.Add("Find which combination of Rocket Values causes the greatest number of changes in the flight path  ");
        scenario2Questions.Add("Find which combination of Rocket Values causes the least number of changes in the flight path  ");

        questionsList.Add(scenario1Questions);
        questionsList.Add(scenario2Questions);
    }

    public void NextScenario()
    {
        saveData();
        currentScenario++;
        
        scenarioTitle.text = "Scenario " + (currentScenario + 1) + " Questions";
        currentScenarioQuestions = questionsList[currentScenario];
        currentQuestion = 0;
        times = new float[currentScenarioQuestions.Count];
   
        setQuestionText();
    }

    private void setQuestionText()
    {
        questionTitle.text = "Question " + (currentQuestion + 1) + ":";
        questionText.text = currentScenarioQuestions[currentQuestion];  
    }

    public void NextQuestion()
    {     
        currentQuestion++;

        if (!prevButton.interactable)
        {
            prevButton.interactable = true;
        }

        if (currentQuestion == (currentScenarioQuestions.Count - 1))
        {
            nextButton.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(true);
        }
        setQuestionText();
        resetToggle();
    }

    public void PreviousQuestion()
    {    
        if (currentQuestion == (currentScenarioQuestions.Count - 1))
        {
            nextButton.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(false);
        }

        currentQuestion--;

        if (currentQuestion == 0)
        {
            prevButton.interactable = false;
        }

        setQuestionText();
        resetToggle();
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

    private void saveData()
    {
        for (int i = 0; i < times.Length; i++)
        {
            Debug.Log("Question " + (i + 1) + "\nAnswer: " + answers[i] + "  Time: " + times[i]);
        }
    }
}
