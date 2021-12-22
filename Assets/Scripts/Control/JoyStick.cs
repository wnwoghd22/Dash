using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Image back;
    private Image stick;
    public bool Down { get; private set; }
    public bool Hold { get; private set; }
    public bool Up { get; private set; }
    private bool isPressed;
    public Vector2 InputDir { get; private set; }
    float backRadius;

    // Start is called before the first frame update
    void Start()
    {
        Down = false;
        Hold = false;
        Up = false;
        back = GetComponent<Image>();
        stick = transform.GetChild(0).GetComponent<Image>();
        backRadius = back.rectTransform.sizeDelta.x / 2;
    }

    // Update is called once per frame
    void Update()
    {
        // low fidelity
        if (!Up && (Hold && Down)) Down = false;
        if (!Down && (!Hold && Up)) Up = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Vector2.zero;

        if (Hold && RectTransformUtility.ScreenPointToLocalPointInRectangle(back.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
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
        //Vector2 pos;
        // I want to set Hold true if and only if player touches stick UI
        //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(stick.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        //{
        //    Hold = true;
        //    Down = true;
        //    Debug.Log("touch stick");
        //}
        //if (Hold)
        //    OnDrag(eventData);
        Hold = true;
        Down = true;
        OnDrag(eventData);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Hold = false;
        InputDir = Vector2.zero;
        stick.rectTransform.anchoredPosition = Vector2.zero;
        Up = true;
    }
}
