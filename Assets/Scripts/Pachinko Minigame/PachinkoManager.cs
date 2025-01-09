using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;

public enum states
{
    startAim,
    aim,
    game,
    next,
    finish
}

public class PachinkoManager : MonoBehaviour
{
    #region Variables
    [Header("Pachinko Params")]
    [SerializeField] private GameObject _pachinkoBall;
    [SerializeField] private GameObject _pachinkoGrid;
    [SerializeField] private GameObject[] _pachinkoLayoutsList;
    [SerializeField] private GameObject _pachinkoEndingWall;
    [SerializeField] private GameObject _pachinkoClickArea;
    [SerializeField] private Material _pachinkoGradient;
    [SerializeField] private float _forceFactor = 2f;
    [SerializeField] private float _minForce = 1f;
    [SerializeField] private float _maxForce = 5f;
    private states _pachinkoState;
    private Rigidbody2D _rb;
    private Vector2 _originalBallPosition;
    private Vector2 _aimDirection;
    private uint _ballCount;
    private uint _maxBalls = 3;
    
    [Header("Color Params")]
    [SerializeField]private SpriteRenderer _targetColorSprite;
    [SerializeField]private RectTransform _valueSelected;
    [SerializeField]private RectTransform _colorSelected;
    [SerializeField]private float _valueSelectedAnimTime;
    private float _originalValuePosY;
    private RGB255 _targetColor;
    private byte[] _RGBComponents;

    [Header("Arrow Params")]
    [SerializeField] private GameObject _arrow;
    [SerializeField] private SpriteRenderer _arrowSprite;
    [SerializeField] private float _minWidth = 4f;
    [SerializeField] private float _maxWidth = 10f;
    #endregion

    #region Singleton
    public static PachinkoManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    #region UnityFunctions
    private void Start()
    {
        _targetColor = RGB255.Random();
        _targetColorSprite.color = _targetColor.ToColor();
        _pachinkoState = states.startAim;
        _originalBallPosition = _pachinkoBall.transform.position;
        _originalValuePosY = _valueSelected.position.y;
        _ballCount = 0;
        _RGBComponents = new byte[3];
        _arrow.SetActive(false);
        _valueSelected.gameObject.SetActive(false);
        _pachinkoClickArea.GetComponent<ClickAreaScript>().pointerDown += ActivateArrow;
        _pachinkoClickArea.GetComponent<ClickAreaScript>().pointerUp += ReleaseArrow;
        Instantiate(_pachinkoLayoutsList[Random.Range(0, _pachinkoLayoutsList.Length)], _pachinkoGrid.transform);
    }

    private void Update()
    {
        switch(_pachinkoState)
        {
            case states.startAim:
                StartAim();
                break;
            case states.aim:
                Aim();
                break;
            case states.game:
                break;
        }
    }

    private void OnDestroy()
    {
        if(_pachinkoClickArea != null)
        {
            _pachinkoClickArea.GetComponent<ClickAreaScript>().pointerDown -= ActivateArrow;
            _pachinkoClickArea.GetComponent<ClickAreaScript>().pointerUp -= ReleaseArrow;
        }
    }
    #endregion

    #region StatesFunctions
    private void StartAim()
    {
        _rb = _pachinkoBall.GetComponent<Rigidbody2D>();
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _valueSelected.gameObject.SetActive(false);

        switch (_ballCount)
        {
            case 0:
                _pachinkoGradient.SetColor("_RightColor", Color.red);
                break;
            case 1:
                _pachinkoGradient.SetColor("_RightColor", Color.green);
                break;
            case 2:
                _pachinkoGradient.SetColor("_RightColor", Color.blue);
                break;
        }
        _pachinkoState = states.aim;
    }

    private void Aim()
    {
        if (_pachinkoClickArea.GetComponent<ClickAreaScript>().touching)
        {
            MoveArrow();
        }
    }

    private void ActivateArrow()
    {
        if (_pachinkoState != states.aim)
            return;
        _arrow.SetActive(true);
    }

    private void MoveArrow()
    {
        var touchPos = _pachinkoClickArea.GetComponent<ClickAreaScript>().touchPosition;
        touchPos = Camera.main.ScreenToWorldPoint(touchPos);
        _aimDirection = touchPos - _originalBallPosition;
        float dirLengthClamped = Mathf.Clamp(_aimDirection.magnitude, _minWidth, _maxWidth);
        Vector2 newSize = new Vector2(dirLengthClamped + Mathf.Clamp(_aimDirection.magnitude, 0f, _maxWidth), _arrowSprite.size.y);
        _arrowSprite.size = newSize;
        float r = (dirLengthClamped - _minWidth) / (_maxWidth - _minWidth);
        float g = 1 - ((dirLengthClamped - _minWidth) / (_maxWidth - _minWidth));
        _arrowSprite.color = new Color(r, g, 0f);
        _arrow.transform.right = _aimDirection;
    }

    private void ReleaseArrow()
    {
        if (_pachinkoState != states.aim)
            return;
        _arrow.SetActive(false);
        _rb.constraints = RigidbodyConstraints2D.None;
        _rb.AddForce(_aimDirection * _forceFactor, ForceMode2D.Impulse);
        _pachinkoState = states.game;
    }

    public void DetectRGBValue()
    {
        ServiceLocator.Get<IMusicService>().PlaySoundPitch("sound");
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        float value = _pachinkoBall.transform.position.x;
        value = value + _pachinkoEndingWall.transform.localScale.x / 2;
        value = Mathf.Clamp(value / _pachinkoEndingWall.transform.localScale.x, 0f, 1f);
        value *= 255;
        _RGBComponents[_ballCount] = (byte)value;
        
        StartCoroutine(ValueSelected());
    }

    private IEnumerator ValueSelected()
    {
        _valueSelected.gameObject.SetActive(true);
        _valueSelected.GetComponentInChildren<TextMeshProUGUI>().text = $"{_RGBComponents[_ballCount]}";
        LeanTween.move(_valueSelected.gameObject, _colorSelected.position, _valueSelectedAnimTime).setEase(LeanTweenType.easeInOutCubic);
        
        yield return new WaitForSeconds(_valueSelectedAnimTime/2);

        LeanTween.scale(_valueSelected, Vector2.one * 0.1f, _valueSelectedAnimTime/2).setEase(LeanTweenType.easeInOutCubic);

        yield return new WaitForSeconds(_valueSelectedAnimTime / 2);

        _ballCount++;
        _colorSelected.GetComponentInChildren<TextMeshProUGUI>().text = $"{_RGBComponents[0]} {_RGBComponents[1]} {_RGBComponents[2]}";
        if (_ballCount < _maxBalls)
        {
            Reset();
            _pachinkoState = states.startAim;
        }
        else
        {
            Reset();
            RGB255 guessedColor = new RGB255(_RGBComponents[0], _RGBComponents[1], _RGBComponents[2]);
            ServiceLocator.Get<IDayService>().FinishMinigame(_targetColor, guessedColor);
        }
    }

    private void Reset()
    {
        GetComponent<GravityManager>().ResetGravity();
        _pachinkoBall.transform.position = _originalBallPosition;
        Vector2 resetPosition = new Vector2(_valueSelected.position.x, _originalValuePosY);
        LeanTween.scale(_valueSelected, Vector2.one, 0f);
        _valueSelected.position = resetPosition;
    }
    #endregion
}