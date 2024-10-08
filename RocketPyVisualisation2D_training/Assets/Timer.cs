using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public int currentQuestion = 1;
    private float currentTime1 = 0;
    private float currentTime2 = 0;
    private float currentTime3 = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentQuestion)
        {
            case 0:
                break;
            case 1:
                currentTime1 += Time.deltaTime;
                break;

            case 2:
                currentTime2 += Time.deltaTime;
                break;

            case 3:
                currentTime3 += Time.deltaTime;
                break;

        }
        
    }
}
