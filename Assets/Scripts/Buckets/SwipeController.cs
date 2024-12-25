using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour, IEndDragHandler
{
    [field: SerializeField] public Vector3 ItemStep { get; private set; }
    [SerializeField] private RectTransform _itemsRect;
    [SerializeField] private float _tweenTime;
    [SerializeField] private LeanTweenType _tweenType;
    
    [Header("Navigation")]
    [SerializeField] private RectTransform _navigationRect;
    [SerializeField] private Image _indicatorPrefab;
    [SerializeField] private Sprite _selectedSprite, _deselectedSprite;
    
    
    private int _maxItem;
    private int _currentItem;
    private Vector3 _targetPos;
    private float _dragThreshold;

    private Image[] _navigationIndicators;
    
    
    public void Initialize(int numberOfItems)
    {
        _maxItem = numberOfItems;
        _currentItem = 1;
        _targetPos = _itemsRect.localPosition;
        _dragThreshold = Screen.width / 15f;

        _navigationIndicators = new Image[numberOfItems];
        for (int i = 0; i < numberOfItems; i++)
        {
            _navigationIndicators[i] = Instantiate(_indicatorPrefab, _navigationRect);
            _navigationIndicators[i].sprite = i == 0 ? _selectedSprite : _deselectedSprite;
        }
    }

    
    public void Next()
    {
        if (_currentItem < _maxItem)
        {
            _currentItem++;
            _targetPos += ItemStep;
            MovePage();
        }
    }
    public void Previous()
    {
        if (_currentItem > 1)
        {
            _currentItem--;
            _targetPos -= ItemStep;
            MovePage();
        }
    }
    private void MovePage()
    {
        _itemsRect.LeanMoveLocal(_targetPos, _tweenTime).setEase(_tweenType);

        UpdateNavigation();
    }

    private void UpdateNavigation()
    {
        for (int i = 0; i < _maxItem; i++)
        {
            _navigationIndicators[i].sprite = i+1 == _currentItem ? _selectedSprite : _deselectedSprite;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs (eventData.position.x - eventData.pressPosition.x) > _dragThreshold)
        {
            if (eventData.position.x > eventData.pressPosition.x) Previous();
            else Next();
        }
        else
        {
            MovePage();
        }
    }
}
