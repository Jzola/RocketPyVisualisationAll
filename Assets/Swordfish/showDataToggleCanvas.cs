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
    //this is the resizer button on the graph config canvas
    public Button btn;
    //Additional listeners are required to minimize the data panel when the 'parent' canvas panel is clicked.
    public Button maximizeButton;
    public Button hideButton;
    private Vector3 zero;
    private bool parentResizedSmall = false;
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
        hideButton.onClick.AddListener(graphConfigHidden);
        maximizeButton.onClick.AddListener(graphConfigHidden);


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
    public void graphConfigHidden()
    {
        //by default the parent canvas is shown. Hide button clicked, then maximize button clicked will activate this function
        parentResizedSmall = !parentResizedSmall;
        //if the data panel was visible, and we are hiding parent, hide data panel now
        if (shown && parentResizedSmall)
        {
            //hide
            displayRect.localScale = zero;
            
        }
        //otherwise if the data panel should be shown
        else if(shown && !parentResizedSmall)
        {
            //put it back
            displayRect.localScale = originalSize;
            ScrollRect srect = dataToggleDisplay.GetComponentInChildren<ScrollRect>();
            ScrollToTop(srect);
        }
        
    }

    public void ScrollToTop(ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }
}
