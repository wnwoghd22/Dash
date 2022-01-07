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
    private bool hold;
    public eButtonState State { get; private set; }

    void Start()
    {
        hold = false;
        State = eButtonState.None;
    }

    void Update()
    {
        switch (State)
        {
            case eButtonState.None:
                break;
            case eButtonState.Down:
                if (hold)
                    State = eButtonState.Pressed;
                break;
            case eButtonState.Pressed:
                break;
            case eButtonState.Up:
                if (!hold)
                    State = eButtonState.None;
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        hold = true;

        if (State == eButtonState.None)
            State = eButtonState.Down;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        hold = false;

        if (State == eButtonState.Pressed)
            State = eButtonState.Up;
    }
}
