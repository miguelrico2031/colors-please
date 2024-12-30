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
    [SerializeField] private states _pachinkoState;
    [SerializeField] private GameObject _pachinkoBall;
    [SerializeField] private GameObject[] _pachinkoLayouts;
    [SerializeField] private GameObject _pachinkoGrid;
    [SerializeField] private SpriteRenderer _targetColorSprite;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Vector2 _originalPosition;
    [SerializeField] private Vector2 _aimDirection;
    [SerializeField] private float _forceFactor;
    [SerializeField] private uint _ballCount;
    [SerializeField] private byte[] _RGBComponents;
    [SerializeField] private uint _maxBalls = 3;

    private void Start()
    {
        RGB255 targetColor = RGB255.Random();
        _targetColorSprite.color = targetColor.ToColor();
        _pachinkoState = states.startAim;
        _ballCount = 0;
        _RGBComponents = new byte[3];
        Instantiate(_pachinkoLayouts[Random.Range(0, _pachinkoLayouts.Length)], _pachinkoGrid.transform);
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
                Game();
                break;
            case states.next:
                break;
            case states.finish:
                break;
        }
    }

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
                var touchPos = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);
                _aimDirection = touchPos - _originalPosition;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                var touchPos = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);
                _aimDirection = touchPos - _originalPosition;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                _rb.constraints = RigidbodyConstraints2D.None;
                _rb.AddForce(_aimDirection * _forceFactor, ForceMode2D.Impulse);
            }
        }
    }

    private void Game()
    {

    }
}