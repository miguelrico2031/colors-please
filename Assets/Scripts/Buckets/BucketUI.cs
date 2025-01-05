using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BucketUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [field: SerializeField] public Bucket Bucket { get; private set; }
    
    public enum State
    {
        Available,
        Unavailable,
        BeingSelected,
        Selected
    }
    public State CurrentState { get; private set; } = State.Available;
    
    public event Action OnInit;
    public event Action OnSelect;

    public TextMeshProUGUI CostText => _costText;

    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Color _disabledColor;
    [FormerlySerializedAs("_boughtColor")] [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _disabledColorCostText;
    [FormerlySerializedAs("_boughtColorCostText")] [SerializeField] private Color _selectedColorCostText;
    
    [Header("Shrink on Scroll Away")]
    [SerializeField] private float _minScale;
    [SerializeField] private float _maxScale;
    [SerializeField] private float _minScaleDistanceFromCenter;

    [Header("Select Confirmation")] 
    [SerializeField] private Image _fill;
    [SerializeField] private float _confirmationDuration;



    
    
    private Image _image;
    private RectTransform _rectTransform;
    private RectTransform _center;
    private Color _defaultColor, _defaultColorCostText;
    private float _confirmationProgress;
    
    
    public void Initialize(Bucket bucket)
    {
        Bucket ??= bucket; //porque el bucket de la hucha se asigna en el inspector y el argumento de initialize es null
        _descriptionText.text = Bucket.Description;
        
        _rectTransform = GetComponent<RectTransform>();
        _center = GameObject.FindWithTag("BucketUICenter").GetComponent<RectTransform>();
        
        UpdateSize();
        
        _fill.fillAmount = 0f;
        
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
        _defaultColorCostText = _costText.color;

        
        OnInit?.Invoke();
    }


    private void Update()
    {
        if (transform.hasChanged)
        {
            UpdateSize();
            transform.hasChanged = false;
        }

        if (CurrentState is State.BeingSelected)
        {
            _confirmationProgress = Mathf.Clamp01(_confirmationProgress + Time.deltaTime / _confirmationDuration);
            _fill.fillAmount = _confirmationProgress;
            if (_confirmationProgress >= 1f)
            {
                //seleccion
                SelectBucket();
            }
        }
    }

    
    private void UpdateSize()
    {
        if (_rectTransform is null || _center is null)
            return;

        float distanceFromCenter = Mathf.Abs(_rectTransform.position.x - _center.position.x);

        float scale = Mathf.Lerp(_maxScale, _minScale, distanceFromCenter / _minScaleDistanceFromCenter);
        scale = Mathf.Clamp(scale, _minScale, _maxScale);

        _rectTransform.localScale = new Vector3(scale, scale, 1);

    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (CurrentState is not State.Available) return;

        CurrentState = State.BeingSelected;
        _confirmationProgress = 0f;
        GetComponent<Animator>().speed = 0f;
        _fill.transform.parent.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
    }

    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (CurrentState is not State.BeingSelected) return;

        Enable();
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (CurrentState is not State.BeingSelected) return;

        Enable();
    }

    


    public void Enable()
    {
        CurrentState = State.Available;
        GetComponent<Animator>().speed = 1f;
        _image.color = _defaultColor;  
        _costText.color = _defaultColorCostText;
        _fill.fillAmount = 0f;
    }

    public void Disable()
    {
        CurrentState = State.Unavailable;
        GetComponent<Animator>().speed = 1f;
        _image.color = _disabledColor;
        _costText.color = _disabledColorCostText;
        _fill.fillAmount = 0f;
    }

    public void DisableEndScene()
    {
        CurrentState = State.Unavailable;
        GetComponent<Animator>().speed = 1f;
        _fill.fillAmount = 0f;
    }

    
     private void SelectBucket()
    {
        CurrentState = State.Selected;
        GetComponent<Animator>().speed = 1f;

        _image.color = _selectedColor;
        _costText.color = _selectedColorCostText;
        _fill.fillAmount = 0f;
        
        OnSelect?.Invoke();
    }
}