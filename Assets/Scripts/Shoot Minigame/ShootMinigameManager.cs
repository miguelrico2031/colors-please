using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootMinigameManager : MonoBehaviour
{
    [SerializeField] private ShootBall _ball;
    [SerializeField] private ShootTarget _target;
    [SerializeField] private Image _targetColorImage;
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private int _countdownTimePerCoord;
    [SerializeField] private float _countdownAnimationTime;
    [SerializeField] private float _countdownAnimationSize;
    [SerializeField] private LeanTweenType _countdownAnimationType;
    [SerializeField] private float _timeBetwwenCoords;
    
    private Queue<RGB255.Coordinate> _coords = new (new [] { RGB255.Coordinate.RED , RGB255.Coordinate.GREEN, RGB255.Coordinate.BLUE});

    private RGB255 _targetColor;
    private Coroutine _countdownCoroutine;
    

    private void Start()
    {
        _targetColor = RGB255.Random();
        _targetColorImage.color = _targetColor.ToColor();

        _ball.OnStop += OnBallStopped;
        _ball.OnShoot += OnBallShot;
        
        StartCoroutine(NextCoord());
    }
    
    private IEnumerator NextCoord()
    {
        ServiceLocator.Get<IMusicService>().PlaySound("aceptar");
        if (_coords.TryDequeue(out var coord))
        {
            yield return new WaitForSeconds(_timeBetwwenCoords);
            _target.StartCoordinate(coord);
            _ball.StartCoordinate(coord, _countdownTimePerCoord);
            if(_countdownCoroutine is not null) StopCoroutine(_countdownCoroutine);
            _countdownCoroutine = StartCoroutine(Countdown());
        }
        else
        {
            _ball.OnStop -= OnBallStopped;
            _ball.OnShoot -= OnBallShot;
            ServiceLocator.Get<IDayService>().FinishMinigame(_targetColor, _ball.GuessedColor);
        }
    }

    private void OnBallStopped()
    {
        StartCoroutine(NextCoord());
    }

    private void OnBallShot()
    {
        if(_countdownCoroutine is not null) StopCoroutine(_countdownCoroutine);
        _countdownText.text = "";
    }
    
    

    private IEnumerator Countdown()
    {
        ServiceLocator.Get<IMusicService>().PlaySound("bip");
        for (int i = _countdownTimePerCoord; i >= 0; i--)
        {
            _countdownText.text = $"{i}";
            _countdownText.rectTransform.localScale = Vector3.one * _countdownAnimationSize;
            LeanTween.scale(_countdownText.gameObject, Vector3.one, _countdownAnimationTime)
                .setEase(_countdownAnimationType);
            yield return new WaitForSeconds(1f);
            ServiceLocator.Get<IMusicService>().PlaySoundPitch("bip", 1/i*2);
        }
    }
}
