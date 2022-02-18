using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Image back;
    private Image stick;

    private bool hold;
    public eButtonState State { get; private set; }

    public Vector2 InputDir { get; private set; }
    float backRadius;

    // Start is called before the first frame update
    void Start()
    {
        hold = false;
        State = eButtonState.None;

        back = GetComponent<Image>();
        stick = transform.GetChild(0).GetComponent<Image>();
        backRadius = back.rectTransform.sizeDelta.x / 2;
    }

    // Update is called once per frame
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

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Vector2.zero;

        if (hold && RectTransformUtility.ScreenPointToLocalPointInRectangle(back.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x /= backRadius * 2;
            pos.y /= backRadius * 2;
            InputDir = new Vector2(pos.x, pos.y);
            InputDir = InputDir.magnitude > 1 ? InputDir.normalized : InputDir;

            Vector2 stickPos = new Vector2(InputDir.x * backRadius * 2, InputDir.y * backRadius * 2);

            stick.rectTransform.anchoredPosition = stickPos.magnitude < backRadius ? stickPos : stickPos * (backRadius / stickPos.magnitude);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);

        hold = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        hold = false;

        InputDir = Vector2.zero;
        stick.rectTransform.anchoredPosition = Vector2.zero;
    }
}
