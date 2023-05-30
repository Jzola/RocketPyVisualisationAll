using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaximizeButton : MonoBehaviour
{
    //WARNING: to use this component with on another canvas, they must be siblings, or not connected at all in component hierarchy.
    //Scaling or deactivating a parent object will always deactivate the child objects.
    /*
     * The minimize button prefab can be found in  Assets/Swordfish/Resources/Prefabs, and will need
     * to be positioned appropriately (and possibly rezised) as a child object on each canvas.
     */

    //Will need to use the inspector to drag and drop the components.
    public GameObject otherPanel;
    public Button otherPanelMinimizeButton;
    
    private bool maximizedStatus = true;

    //the original size of the canvas, for restoring the menu.
    private RectTransform rectThis;
    private RectTransform rectThat;
    private Vector3 originalScaleMaximizer;
    private Vector3 originalScaleOtherPanel;
    private Vector3 zero;
    private Button maxButton;
    private Text heading;
    void Start()
    {
        zero = new Vector3(0, 0, 0);
        //get original scale value of this canvas. A canvas or a cube uses a rect transform
        rectThis = transform.gameObject.GetComponent<RectTransform>();      
        //store the starting scale
        originalScaleMaximizer = rectThis.localScale;

        //get the original size of the menu to be minimized
        rectThat = otherPanel.GetComponent<RectTransform>();
        originalScaleOtherPanel = rectThat.localScale;

        //start by minimizing the component so it won't be visible or interactable
        rectThis.localScale = zero;

        //there should only be one button to find.
        maxButton = this.gameObject.GetComponentInChildren<Button>();
        heading = this.gameObject.transform.GetChild(0).GetComponent<Text>();

        //add the listener to both buttons
        otherPanelMinimizeButton.onClick.AddListener(ToggleCanvasVisibility);
        maxButton.onClick.AddListener(ToggleCanvasVisibility);


        
         
    }
    [ContextMenu("Toggle Panel Size")]
    public void ToggleCanvasVisibility()
    {
        if (maximizedStatus)
        {
            //the panel is big, we need to make it 0, 0,0, and expand the size of the maximizer.
            rectThis.localScale = originalScaleMaximizer;
            rectThat.localScale = zero;

            maximizedStatus = false;
        }
        else
        {
            rectThis.localScale = zero;
            rectThat.localScale = originalScaleOtherPanel;
            maximizedStatus = true;
        }

    }

    //purely for visual reasons, ensure that the heading fits in the panel 
    string fixPanelTextLength(int stringLength, string heading)
    {

        //TODO - shorten text and return it
        return "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
