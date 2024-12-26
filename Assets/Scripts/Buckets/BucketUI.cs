using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BucketUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public Bucket Bucket { get; private set; }
    
    public enum State
    {
        Available,
        Unavailable,
        BeingSelected,
        Bought
    }
    public State CurrentState { get; private set; } = State.Available;

    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private Color _disabledColor;
    [SerializeField] private Color _boughtColor;
    [SerializeField] private Color _disabledColorCostText;
    [SerializeField] private Color _boughtColorCostText;
    
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
    
    
    private IMoneyService _moneyService;
    public void Initialize(Bucket bucket)
    {
        Bucket = bucket;
        _descriptionText.text = Bucket.Description;
        
        _rectTransform = GetComponent<RectTransform>();
        _center = GameObject.FindWithTag("BucketUICenter").GetComponent<RectTransform>();
        
        UpdateSize();
        
        _fill.fillAmount = 0f;
        
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
        _defaultColorCostText = _costText.color;

        _moneyService = ServiceLocator.Get<IMoneyService>();
        _moneyService.OnMoneyChange += OnMoneyChange;
        OnMoneyChange();

        _costText.text = $"Cost: {Bucket.Cost}";
    }

    private void OnDisable()
    {
        if(_moneyService is not null && CurrentState is not State.Bought)
            _moneyService.OnMoneyChange -= OnMoneyChange;
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

    
    private void OnMoneyChange()
    {
        var totalMoney = _moneyService.DayMoney + _moneyService.PiggyBankMoney;
        if (totalMoney < Bucket.Cost)
        {
            Disable();
        }
        else
        {
            Enable();
        }
    }

    private void Enable()
    {
        CurrentState = State.Available;
        _image.color = _defaultColor;  
        _costText.color = _defaultColorCostText;
        _fill.fillAmount = 0f;
    }

    private void Disable()
    {
        CurrentState = State.Unavailable;
        _image.color = _disabledColor;
        _costText.color = _disabledColorCostText;
        _fill.fillAmount = 0f;
    }

    
    private void SelectBucket()
    {
        CurrentState = State.Bought;
        _moneyService.OnMoneyChange -= OnMoneyChange;

        _image.color = _boughtColor;
        _costText.color = _boughtColorCostText;
        _costText.text = "Bought";
        _fill.fillAmount = 0f;
        
        if (Bucket.Cost <= _moneyService.DayMoney)
        {
            _moneyService.RemoveDayMoney(Bucket.Cost);
        }
        else
        {
            uint dayMoney = _moneyService.DayMoney;
            _moneyService.RemoveDayMoney(dayMoney);
            uint remainder = Bucket.Cost - dayMoney;
            _moneyService.RemoveFromPiggyBank(remainder);
        }

        var relationShipService = ServiceLocator.Get<IRelationshipService>();

        foreach (var consequence in Bucket.PayedConsequences)
        {
            uint points = (uint)Mathf.Abs(consequence.Points);
            if (consequence.Points > 0)
            {
                relationShipService.AddPoints(consequence.character, points);
            }
            else
            {
                relationShipService.RemovePoints(consequence.character, points);
            }
        }
    }
}