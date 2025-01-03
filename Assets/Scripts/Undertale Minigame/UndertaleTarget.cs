
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UndertaleTarget : MonoBehaviour, IPointerDownHandler
{
    public event Action OnNeedleStopped;
    public RGB255 GuessedColor { get; private set; } = new RGB255();
    
    private static readonly int _leftColorHash = Shader.PropertyToID("_LeftColor");
    private static readonly int _rightColorHash = Shader.PropertyToID("_RightColor");
    [SerializeField] private RectTransform _needle;
    [SerializeField] private Image _backgroundGradient;
    [SerializeField] private float _horizontalBound;
    [SerializeField] private float _needlePeriod;
    [SerializeField] private LeanTweenType _easeType;

    private bool _isNeedleMoving;
    private RGB255.Coordinate _currentCoord;
    private float _countdownTimer;
    private int _tweenID;
    
    private Vector2 _needleStartPosition;


    private void Awake()
    {
        _needleStartPosition = _needle.anchoredPosition;
    }

    private void Update()
    {
        if (!_isNeedleMoving) return;
        
        _countdownTimer -= Time.deltaTime;
        if (_countdownTimer <= 0)
        {
            StopNeedle();
        }
    }

    public void StartCoordinate(RGB255.Coordinate coord, float maxGuessTime)
    {
        _currentCoord = coord;
        _isNeedleMoving = true;
        _countdownTimer = maxGuessTime;
        
        var leftGradientColor = new RGB255().ToColor();
        var rightGradientColor = new RGB255().SetCoordinate(coord, 255).ToColor();
        
        _backgroundGradient.material.SetColor(_leftColorHash, leftGradientColor);
        _backgroundGradient.material.SetColor(_rightColorHash, rightGradientColor);
        

        Vector2 rightBound = _needleStartPosition + Vector2.right * _horizontalBound;
        Vector2 leftBound = _needleStartPosition + Vector2.left * _horizontalBound;

        _tweenID = LeanTween.value(_needle.gameObject, (pos) => _needle.anchoredPosition  = pos, leftBound, rightBound, _needlePeriod)
            .setFrom(leftBound)
            .setLoopPingPong()
            .setEase(_easeType)
            .id; 
    }
    
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isNeedleMoving)
            StopNeedle();
    }


    private float GetNeedleProgress()
    {
        return Mathf.InverseLerp(-_horizontalBound, _horizontalBound, _needle.anchoredPosition.x);
    }


    private void StopNeedle()
    {
        float progress = GetNeedleProgress();
        _isNeedleMoving = false;
        LeanTween.cancel(_tweenID);
        GuessedColor = GuessedColor.SetCoordinate(_currentCoord, (byte) Mathf.RoundToInt(progress * 255f));
        OnNeedleStopped?.Invoke();
    }
}
