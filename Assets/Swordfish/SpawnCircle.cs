using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnCircle : MonoBehaviour
{
    public enum State { DEFAULT = 0, SELECTED = 1, IN_USE = 2}

    public int ID;
    public GraphSpawnHandler handler;
    private Image graphic;
    public Color[] colours; // 0 = default, 1 = selected, 2 = in use
    private State currentState = State.DEFAULT;

    
    // Start is called before the first frame update
    void Start()
    {
        graphic = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        graphic.color = colours[(int)currentState];
    }

    [ContextMenu("Select")]
    public void select()
    {
        handler.selectCircle(ID);
    }

    public void changeState(State state)
    {
        this.currentState = state;
    }
}
