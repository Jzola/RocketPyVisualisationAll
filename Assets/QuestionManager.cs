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
    private int[] answers;
    private float[] times;
    private bool running;
    
    private List<string> questions = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        toggles = new List<Toggle>(answerToggles.GetComponentsInChildren<Toggle>());
        toggleGroup = answerToggles.GetComponent<ToggleGroup>();
        toggleText = new List<Text>(answerToggles.GetComponentsInChildren<Text>());

        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener(delegate
            {
                checkToggle(toggle);
            });
        }


        questions.Add("Question 1: aaaaaa");
        questions.Add("Question 2: bbbbbbbbbb");
        questions.Add("Question 3: cccccccccc");
        questions.Add("Question 4: aaaaaa");
        questions.Add("Question 5: bbbbbbbbbb");

        questionText.text = questions[0];

        times = new float[questions.Count];
        answers = new int[questions.Count];
        running = true;
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

    public void NextQuestion()
    {
        Debug.Log("Question " + (currentQuestion + 1) + " Answer: " + answers[currentQuestion] + " Time: " + times[currentQuestion]);

        if (answers[currentQuestion] == 0)
            return;

        currentQuestion++;
        questionText.text = questions[currentQuestion];
        if (currentQuestion == questions.Count - 1)
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
        questionText.text = questions[currentQuestion];

        if (!nextButton.IsActive())
        {
            nextButton.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(false);
        }

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
