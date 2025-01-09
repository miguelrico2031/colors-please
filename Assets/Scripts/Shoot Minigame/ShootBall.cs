
using System;
using UnityEngine;

public class ShootBall : MonoBehaviour
{
    [SerializeField] private Collider2D _targetCollider;
    [SerializeField] private ClickAreaScript _clickArea;
    [SerializeField] private Transform _arrow;
    [SerializeField] private float _minArrowScale, _maxArrowScale, _arrowScaleFactor;
    [SerializeField] private float _forceFactor;
    [SerializeField] private float _colliderDisabledTime = .3f;
    public event Action OnShoot;
    public event Action OnStop;
    public RGB255 GuessedColor {get; private set;} = new RGB255();

    private Rigidbody2D _rb;
    private RGB255.Coordinate _currentCoordinate;
    private bool _isAiming;
    private Vector2 _touchPos;
    private Transform _arrowChild;
    private Vector2 _defaultArrowScale;
    private float _aimCountdown;
    private Vector3 _defaultPosition;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _targetCollider = GetComponent<Collider2D>();
        _arrowChild = _arrow.GetChild(0);
        _defaultArrowScale = _arrowChild.localScale;
        _clickArea.pointerUp += Shoot;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _arrow.gameObject.SetActive(false);
        _defaultPosition = transform.position;
    }

    private void OnDisable()
    {
        _clickArea.pointerUp -= Shoot;
    }

    private void Update()
    {
        if (_isAiming)
        {
            Aim();
            _aimCountdown -= Time.deltaTime;
            if (_aimCountdown <= 0)
            {
                Shoot();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (_isAiming) return;
        if (other.collider.TryGetComponent<ShootTarget>(out var target))
        {
            GuessedColor = GuessedColor.SetCoordinate(_currentCoordinate, 
                (byte) Mathf.RoundToInt(target.GetProgress(transform.position.x) * 255f));
            Stop();
        }
        else if (other.gameObject.name == "Ground")
        {
            GuessedColor = GuessedColor.SetCoordinate(_currentCoordinate, 0);
            transform.position = _defaultPosition;
            Stop();
        }
    }
    
    public void StartCoordinate(RGB255.Coordinate coordinate, float maxAimTime)
    {
        _currentCoordinate = coordinate;
        _isAiming = true;
        _arrow.gameObject.SetActive(true);
        var scale = _defaultArrowScale;
        scale.x = _minArrowScale;
        _arrowChild.localScale = scale;
        _arrow.rotation = Quaternion.Euler(0, 0, 0);
        _aimCountdown = maxAimTime;
    }

    private void Aim()
    {
        if (!_clickArea.touching) return;
        if (_touchPos == _clickArea.touchPosition) return;
        
        _touchPos = _clickArea.touchPosition;
        Vector2 direction = transform.position - Camera.main.ScreenToWorldPoint(_touchPos);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _arrow.rotation = Quaternion.Euler(0f, 0f, angle);
        var scale = _defaultArrowScale;
        scale.x = Mathf.Clamp(direction.magnitude * _arrowScaleFactor, _minArrowScale, _maxArrowScale);
        _arrowChild.localScale = scale;
    }

    private void Shoot()
    {
        if (!_isAiming) return;
        _isAiming = false;
        _arrow.gameObject.SetActive(false);
        
        if(_arrow.eulerAngles.z is > 5f and < 175f)
        {
            _targetCollider.enabled = false;
            Invoke(nameof(EnableCollider), _colliderDisabledTime);
        }
        
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.AddForce(_arrow.right * (_arrowChild.localScale.x * _forceFactor), ForceMode2D.Impulse);
        OnShoot?.Invoke();
    }
    
    
    private void EnableCollider() => _targetCollider.enabled = true;

    private void Stop()
    {
        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        OnStop?.Invoke();
    }
}
