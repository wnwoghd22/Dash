using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum eButtonState
{
    None,
    Down,
    Pressed,
    Up,
}

public class JoyButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool Hold { get; private set; }
    public eButtonState State { get; private set; }

    void Start()
    {
        Hold = false;
        State = eButtonState.None;
    }

    void Update()
    {
        switch (State)
        {
            case eButtonState.None:
                break;
            case eButtonState.Down:
                if (Hold)
                    State = eButtonState.Pressed;
                break;
            case eButtonState.Pressed:
                break;
            case eButtonState.Up:
                if (!Hold)
                    State = eButtonState.None;
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Hold = true;

        if (State == eButtonState.None)
            State = eButtonState.Down;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Hold = false;

        if (State == eButtonState.Pressed)
            State = eButtonState.Up;
    }
}
