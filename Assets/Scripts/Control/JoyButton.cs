using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool Hold { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        Hold = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Hold = false;
    }
}
