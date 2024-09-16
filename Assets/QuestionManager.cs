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

    private List<Toggle> toggles;
    private List<Text> toggleText;

    private int currentQuestion = 0;
    private int answer;
    private int[] answers = { 0, 0, 0 };
    private float[] times;
    private bool running;

    private List<string> questions = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        toggles = new List<Toggle>(answerToggles.GetComponentsInChildren<Toggle>());
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

        questionText.text = questions[0];

        times = new float[questions.Count];
        running = true;
    }

    void Update()
    {
        if (running)
            times[currentQuestion] += Time.deltaTime;
    }

    public void NextQuestion()
    {
        Debug.Log("Question " + (currentQuestion + 1) + " Answer: " + answers[currentQuestion] + " Time: " + times[currentQuestion]);

        if (answers[currentQuestion] == 0)
            return;

        currentQuestion++;
        questionText.text = questions[currentQuestion];
        resetToggle();      
    }

    public void PrevQuestion()
    {
        if (currentQuestion == 0)
            return;
        currentQuestion--;
        questionText.text = questions[currentQuestion];
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
        Debug.Log("Question " + currentQuestion + " Answer: " + answers[currentQuestion]);
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
