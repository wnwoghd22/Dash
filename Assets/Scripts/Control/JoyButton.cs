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
                if (hold)
                    State = eButtonState.Down;
                break;
            case eButtonState.Down:
                if (hold)
                    State = eButtonState.Pressed;
                break;
            case eButtonState.Pressed:
                if (!hold)
                    State = eButtonState.Up;
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
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        hold = false;
    }
}
