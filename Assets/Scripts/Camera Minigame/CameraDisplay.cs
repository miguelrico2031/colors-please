using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDisplay : MonoBehaviour, IPointerClickHandler
{
    private CameraEyedropper _eyedropper;

    private bool _hasClicked;
    private Vector2 _moveToPos;

    private void Start()
    {
        _eyedropper = FindAnyObjectByType<CameraEyedropper>();
    }

    private void Update()
    {
        if (!_hasClicked) return;
        
        _hasClicked = false;
        if (!_eyedropper.IsDragging)
        {
            Debug.Log("instamoviendo eyedropper");
            _eyedropper.MoveTo(_moveToPos);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_hasClicked)
        {
            _hasClicked = true;
            _moveToPos = eventData.position;
        }
    }
}