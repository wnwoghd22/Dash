using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Image back;
    private Image stick;
    public Vector2 InputDir { get; private set; }
    float backSizeX;
    float backSizeY;
    float backRadius;

    // Start is called before the first frame update
    void Start()
    {
        back = GetComponent<Image>();
        stick = transform.GetChild(0).GetComponent<Image>();
        backSizeX = back.rectTransform.sizeDelta.x;
        backSizeY = back.rectTransform.sizeDelta.y;
        backRadius = back.rectTransform.sizeDelta.x / 2;
        Debug.Log(backSizeX + ", " + backSizeY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Vector2.zero;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(back.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x /= backSizeX;
            pos.y /= backSizeY;
            InputDir = new Vector2(pos.x, pos.y);
            InputDir = InputDir.magnitude > 1 ? InputDir.normalized : InputDir;

            Vector2 stickPos = new Vector2(InputDir.x * backSizeX, InputDir.y * backSizeY);

            stick.rectTransform.anchoredPosition = stickPos.magnitude < backRadius ? stickPos : stickPos * (backRadius / stickPos.magnitude);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputDir = Vector2.zero;
        stick.rectTransform.anchoredPosition = Vector2.zero;
    }
}
