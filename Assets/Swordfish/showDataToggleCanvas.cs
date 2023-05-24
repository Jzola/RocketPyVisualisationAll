using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class showDataToggleCanvas : MonoBehaviour
{
    public GameObject dataToggleDisplay;
    private RectTransform displayRect;
    private Vector3 originalSize;
    private bool shown = false;
    public Button btn;
    private Vector3 zero;
    // Start is called before the first frame update
    void Start()
    {
        zero = new Vector3(0, 0, 0);
        //get the original size from rect transform.
        displayRect = dataToggleDisplay.GetComponent<RectTransform>();
        originalSize = displayRect.localScale;
        btn.onClick.AddListener(toggleDisplay);
        //hide at start.
        displayRect.localScale = zero;


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ContextMenu("Toggle data vis menu")]
    public void toggleDisplay()
    {
        if (shown)
        {
            //hide
            displayRect.localScale = zero;

        }
        else {
            //show
            displayRect.localScale = originalSize;
            ScrollRect srect = dataToggleDisplay.GetComponentInChildren<ScrollRect>();
            ScrollToTop(srect);

        } 
        shown = !shown;
    }

    public void ScrollToTop(ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }
}
