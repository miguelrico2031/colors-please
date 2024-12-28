
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CameraEyedropper : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public bool IsDragging { get; private set; }
    public RGB255 SelectedColor { get; private set; }
    
    [SerializeField] private WebcamManager _webcamManager;
    [SerializeField] private TextMeshProUGUI _rgbText;
    [SerializeField] private Image _rgbImage;
    [SerializeField] private RectTransform _bounds;
    [SerializeField] private RawImage _cameraDisplayRawImage;
    [SerializeField] private float _boundsPadding;
    [SerializeField] private float _updateColorPeriod = .25f;
    
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private float _updateColorTimer;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _updateColorTimer = _updateColorPeriod;
    }

    private void Update()
    {
        if (!_webcamManager.IsReady) return;
        _updateColorTimer -= Time.deltaTime;
        if (_updateColorTimer > 0f) return;
        _updateColorTimer = _updateColorPeriod;
        UpdateColor();
    }

    public void OnDrag(PointerEventData eventData)
    {
        var newPos = (Vector2)_rectTransform.anchoredPosition + eventData.delta / _canvas.scaleFactor;
        
        newPos.x = Mathf.Clamp(newPos.x, _boundsPadding, _bounds.rect.width - _boundsPadding);
        newPos.y = Mathf.Clamp(newPos.y, _boundsPadding, _bounds.rect.height - _boundsPadding);

        _rectTransform.anchoredPosition = newPos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        IsDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDragging = false;
    }

    public void MoveTo(Vector2 newPos)
    {
        _rectTransform.position = newPos;
        newPos = _rectTransform.anchoredPosition;
                
        newPos.x = Mathf.Clamp(newPos.x, _boundsPadding, _bounds.rect.width - _boundsPadding);
        newPos.y = Mathf.Clamp(newPos.y, _boundsPadding, _bounds.rect.height - _boundsPadding);

        _rectTransform.anchoredPosition = newPos;
    }


    private void UpdateColor()
    {

        var displayRectTransform = _cameraDisplayRawImage.rectTransform;
        
        //pasar a coordenadas en el espacio vectorial de la raw image
        Vector2 displaySpacePosition = displayRectTransform.InverseTransformPoint(_rectTransform.position);
        
        //pasar a coordenadas normalizadas con bounds (-1, -1) y (1, 1)

        Vector2 normalizedPosition = displaySpacePosition;
        normalizedPosition.x /= displayRectTransform.rect.width * 0.5f;
        normalizedPosition.y /= displayRectTransform.rect.height * 0.5f;
        
        //pasar a coordenadas normalizadas con bounds (0, 0) y (1, 1)
        normalizedPosition += Vector2.one;
        normalizedPosition /= 2f;
        
        int texWidth = _webcamManager.WebcamTexture.width;
        int texHeight = _webcamManager.WebcamTexture.height;
        
        Vector2 textureCoordinates = new Vector2(normalizedPosition.x * texWidth, normalizedPosition.y * texHeight);
        int textureX = Mathf.RoundToInt(textureCoordinates.x);
        int textureY = Mathf.RoundToInt(textureCoordinates.y);
        
        Color color = _webcamManager.WebcamTexture.GetPixel(textureX, textureY);
        SelectedColor = new RGB255(color);
        _rgbText.text = $"({SelectedColor.R}, {SelectedColor.G}, {SelectedColor.B})";
        _rgbImage.color = color;
    }
}
