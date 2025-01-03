
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PressArea : MonoBehaviour, IPointerDownHandler
{
    public event Action OnPress;
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPress?.Invoke();
    }
}
