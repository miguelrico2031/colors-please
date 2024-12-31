using Unity.VisualScripting;
using UnityEngine;
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
    [SerializeField] private float _forceFactor = 2f;
    [SerializeField] private float _minForce = 1f;
    [SerializeField] private float _maxForce = 5f;
    [SerializeField] private GameObject[] _pachinkoLayoutsList;
    [SerializeField] private GameObject _pachinkoGrid;
    [SerializeField] private GameObject _pachinkoEndingWall;
    [Header("Target Color")]
    [SerializeField]private SpriteRenderer _targetColorSprite;
    [Header("Arrow Params")]
    [SerializeField] private GameObject _arrow;
    [SerializeField] private SpriteRenderer _arrowSprite;
    [SerializeField] private float _minWidth = 4f;
    [SerializeField] private float _maxWidth = 10f;

    private states _pachinkoState;
    private Rigidbody2D _rb;
    private Vector2 _originalPosition;
    private Vector2 _aimDirection;
    private uint _ballCount;
    private uint _maxBalls = 3;
    private RGB255 _targetColor;
    private byte[] _RGBComponents;
    
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
        _ballCount = 0;
        _RGBComponents = new byte[3];
        _arrow.SetActive(false);
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
            case states.finish:
                break;
        }
    }
    #endregion
    
    #region StatesFunctions
    private void StartAim()
    {
        _rb = _pachinkoBall.GetComponent<Rigidbody2D>();
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        _originalPosition = _pachinkoBall.transform.position;
        _pachinkoState = states.aim;
    }

    private void Aim()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _arrow.SetActive(true);
            }

            if (touch.phase == TouchPhase.Moved)
            {
                var touchPos = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);
                _aimDirection = touchPos - _originalPosition;
                float dirLengthClamped = Mathf.Clamp(_aimDirection.magnitude, _minWidth, _maxWidth);
                Vector2 newSize = new Vector2(dirLengthClamped + Mathf.Clamp(_aimDirection.magnitude, 0f, _maxWidth), _arrowSprite.size.y);
                _arrowSprite.size = newSize;
                float r = (dirLengthClamped - _minWidth) / (_maxWidth - _minWidth);
                float g = 1 - ((dirLengthClamped - _minWidth) / (_maxWidth - _minWidth));
                _arrowSprite.color = new Color(r,g,0f);
                _arrow.transform.right = _aimDirection;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                _arrow.SetActive(false);
                _rb.constraints = RigidbodyConstraints2D.None;
                _rb.AddForce(_aimDirection * _forceFactor, ForceMode2D.Impulse);
                _pachinkoState = states.game;
            }
        }
    }

    public void DetectRGBValue()
    {
        _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        float value = _pachinkoBall.transform.position.x;
        value = value + _pachinkoEndingWall.transform.localScale.x / 2;
        value = Mathf.Clamp(value / _pachinkoEndingWall.transform.localScale.x, 0f, 1f);
        value *= 255;
        _RGBComponents[_ballCount] = (byte) value;
        Debug.Log(_RGBComponents[_ballCount]);
        _ballCount++;
        if( _ballCount < _maxBalls)
        {
            ResetBall();
            _pachinkoState = states.startAim;
        } else
        {
            RGB255 guessedColor = new RGB255(_RGBComponents[0], _RGBComponents[1], _RGBComponents[2]);
            Debug.Log(guessedColor.ToString());
            ServiceLocator.Get<IDayService>().FinishMinigame(_targetColor, guessedColor);
        }
    }

    private void ResetBall()
    {
        _pachinkoBall.transform.position = _originalPosition;
    }
    #endregion
}