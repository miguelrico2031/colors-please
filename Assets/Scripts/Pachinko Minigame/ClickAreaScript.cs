using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickAreaScript : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 touchPosition;
    public bool touching;
    public event Action pointerDown, pointerUp;

    public void OnDrag(PointerEventData eventData)
    {
        touchPosition = eventData.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        touchPosition = eventData.position;
        pointerDown?.Invoke();
        touching = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerUp?.Invoke();
        touching = false;
    }
}
