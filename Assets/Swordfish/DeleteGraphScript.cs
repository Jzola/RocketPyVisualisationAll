using UnityEngine;
using UnityEngine.UI;

public class DeleteGraphScript : MonoBehaviour
{
    private GraphConfig gConfig;
    public Button confirmButton;
    public Button cancelButton;
    public Button launchButton;
    private RectTransform rect;
    private Vector3 deletePanelBigScale;
    private bool launched = true;
    private Vector3 minSize = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        gConfig = transform.parent.gameObject.GetComponent<GraphConfig>();
        

        //get the delete panel's viewing size
        rect = transform.gameObject.GetComponent<RectTransform>();
        deletePanelBigScale = rect.localScale;

        //minimize the delete panel

        toggleDeletePanelVisibility();

        //set up the button listeners
        launchButton.onClick.AddListener(toggleDeletePanelVisibility);
        cancelButton.onClick.AddListener(toggleDeletePanelVisibility);
        confirmButton.onClick.AddListener(deletePressed);

    }
    [ContextMenu("Delete graph")]
    public void deletePressed()
    {
        gConfig.DeleteGraph();
    }

    [ContextMenu("Test panel")]
    public void toggleDeletePanelVisibility()
    {
        if (launched)
        {
            rect.localScale = minSize;
            launched = false;
        }
        else
        {
            rect.localScale = deletePanelBigScale;
            launched = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
