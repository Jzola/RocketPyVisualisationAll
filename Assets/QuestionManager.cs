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
        scenario1Questions.Add("Mass: How does increasing the mass affect the altitude and duration of the rocket's flight path?");
        scenario1Questions.Add("Top Radius: How does changing the top radius impact the trajectory?");
        scenario1Questions.Add("Tail Length: How does adjusting the length of the tail affect the rocket’s flight duration and stability? ");
        scenario1Questions.Add("Nose to CM: How does increasing the distance from the nose to the centre of mass affect the performance of the rocket’s flight path? ");
        scenario1Questions.Add("Tail to CM: How does altering the distance from the tail to the centre of mass affect the performance of the rocket’s flight path? ");

        List<string> scenario2Questions = new List<string>();
        scenario2Questions.Add("Mass & Tail Length: What is the effect of altering the rocket’s mass in combination with the tail length to affect the flight duration, stability, and trajectory? ");
        scenario2Questions.Add("Mass & Nose to CM: How does varying the rocket’s mass in combination with the distance between the nose and the centre of mass affect the stability and trajectory of the rocket? ");
        scenario2Questions.Add("Mass & Tail to CM: What is the combined effect of changing both the rocket mass and the distance between the tail to the centre of mass, affect the rocket’s flight path? ");
        scenario2Questions.Add("Mass & Nozzle: How does variation in the rocket’s mass combined with differing nozzle designs affect the flight path of the rocket? ");
        scenario2Questions.Add("Mass & Propellant: What is the impact of changing both rocket mass and the distance of the propellant, have on the overall flight performance and stability? ");

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
