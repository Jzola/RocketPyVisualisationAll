using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPanel : MonoBehaviour
{
    [SerializeField]
    private Text questionText;
    [SerializeField]
    private GameObject answerToggles;

    private List<Toggle> toggles;
    private List<Text> toggleText;

    public int Answer = 0;

    // Start is called before the first frame update
    void Start()
    {
        toggles = new List<Toggle>(answerToggles.GetComponentsInChildren<Toggle>(true));
        toggleText = new List<Text>(answerToggles.GetComponentsInChildren<Text>(true));

        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener(delegate
            {
                checkToggle(toggle);
            });
        }
    }

    public void SetQuestionText(string text)
    {
        questionText.text = text;
    }

    private void checkToggle(Toggle change)
    {
        int selected = 0;
        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i].isOn)
                selected = i + 1;
        }
        Answer = selected;
    }

    public void ResetToggles()
    {
        foreach (Toggle toggle in toggles)
            toggle.SetIsOnWithoutNotify(false);
    }
}
