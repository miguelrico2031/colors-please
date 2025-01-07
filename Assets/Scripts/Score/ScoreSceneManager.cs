using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSceneManager : MonoBehaviour
{
    [SerializeField] private bool _isEndless;
    [SerializeField] private float _sceneDuration;
    [Header("Remaining time Indicator")]
    [SerializeField] private Image _remainingTimeIndicator;
    [SerializeField] private float _indicatorAnimationMaxScale;
    [SerializeField] private float _indicatorAnimationDuration;
    [SerializeField] private LeanTweenType _indicatorAnimationType;

    [Header("Background Colors")]
    [SerializeField] private Image[] _targetColors;
    [SerializeField] private Image[] _guessedColors;
    [SerializeField] private Image _gradient;
    
    [Header("Colors")]
    [SerializeField] private RectTransform _targetColor;
    [SerializeField] private TextMeshProUGUI _targetColorText;
    [SerializeField] private Image _targetColorImage;
    [SerializeField] private RectTransform _guessedColor;
    [SerializeField] private TextMeshProUGUI _guessedColorText;
    [SerializeField] private Image _guessedColorImage;
    [SerializeField] private float _colorsAnimationInitialScale;
    [SerializeField] private float _colorsAnimationDuration;
    [SerializeField] private LeanTweenType _colorsAnimationType;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI _percentageText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _tipText;
    [SerializeField] private TextMeshProUGUI _totalMoneyText;
    [SerializeField] private float _scoreAnimationInitialScale;
    [SerializeField] private float _scoreAnimationDuration;
    [SerializeField] private LeanTweenType _scoreAnimationType;

    [Header("Endless")]
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _newHighScoreText;

    private static readonly int _topColorHash = Shader.PropertyToID("_TopColor");
    private static readonly int _bottomColorHash = Shader.PropertyToID("_BottomColor");
    private IDayService _dayService;
    private IScoreService _scoreService;
    private float _remainingTime;
    private Score _score;
    private bool _isEndlessGameOver;

    private void Awake()
    {
        _dayService = ServiceLocator.Get<IDayService>();
        _scoreService = ServiceLocator.Get<IScoreService>();
        _remainingTime = _sceneDuration;
        
    }

    private void Start()
    {
        ServiceLocator.Get<IMusicService>().PlaySound("fin");
        InitializeUI();
        StartCoroutine(DisplayScore());
    }

    private void Update()
    {
        _remainingTime -= Time.deltaTime;
        _remainingTimeIndicator.fillAmount = _remainingTime / _sceneDuration;
        if(_remainingTime <= 0f)
            NextMinigame();
    }

    private void InitializeUI()
    {
        foreach (var img in _targetColors)
            img.color = _dayService.TargetColor.ToColor();
        foreach (var img in _guessedColors)
            img.color = _dayService.GuessedColor.ToColor();
        
        _gradient.material.SetColor(_topColorHash, _dayService.TargetColor.ToColor());
        _gradient.material.SetColor(_bottomColorHash, _dayService.GuessedColor.ToColor());
        
        
        _targetColorText.text = $"({_dayService.TargetColor.R}, {_dayService.TargetColor.G}, {_dayService.TargetColor.B})";
        _targetColorText.color = RGB255.GetSimilarity(_dayService.TargetColor, new RGB255()) < .5f
            ? Color.black
            : Color.white;
        _targetColorImage.color = _dayService.TargetColor.ToColor();
        
        _guessedColorText.text = $"({_dayService.GuessedColor.R}, {_dayService.GuessedColor.G}, {_dayService.GuessedColor.B})";
        _guessedColorText.color = RGB255.GetSimilarity(_dayService.GuessedColor, new RGB255()) < .5f
            ? Color.black
            : Color.white;
        _guessedColorImage.color = _dayService.GuessedColor.ToColor();

        _score = _scoreService.GetScore(_dayService.TargetColor, _dayService.GuessedColor);

        _isEndlessGameOver = _isEndless && _score.Percentage < _scoreService.EndlessMinPercentage;
        
        _percentageText.text = $"{_score.Percentage:F2}%";
        _moneyText.text = $"${_score.Money}";
        _tipText.text = $"${_score.Tip}";
        _totalMoneyText.text = $"${_score.Money + _score.Tip}";
        
                
        _targetColor.gameObject.SetActive(false);
        _guessedColor.gameObject.SetActive(false);
        _percentageText.rectTransform.parent.gameObject.SetActive(false);
        _moneyText.rectTransform.parent.gameObject.SetActive(false);
        _tipText.rectTransform.parent.gameObject.SetActive(false);
        _totalMoneyText.rectTransform.parent.gameObject.SetActive(false);
        
        
        _remainingTimeIndicator.fillAmount = 1f;
        LeanTween.scale(_remainingTimeIndicator.gameObject, Vector3.one * _indicatorAnimationMaxScale,
                _indicatorAnimationDuration)
            .setEase(_indicatorAnimationType)
            .setLoopPingPong();
    }


    private IEnumerator DisplayScore()
    {
        yield return new WaitForSeconds(.5f);
        ServiceLocator.Get<IMusicService>().PlaySound("aceptar");

        _targetColor.localScale = Vector3.one * _colorsAnimationInitialScale;
        _guessedColor.localScale = Vector3.one * _colorsAnimationInitialScale;
        
        _targetColor.gameObject.SetActive(true);
        LeanTween.scale(_targetColor.gameObject, Vector3.one, _colorsAnimationDuration)
            .setEase(_colorsAnimationType);
        yield return new WaitForSeconds(_colorsAnimationDuration);
        ServiceLocator.Get<IMusicService>().PlaySound("aceptar");

        _guessedColor.gameObject.SetActive(true);
        LeanTween.scale(_guessedColor.gameObject, Vector3.one, _colorsAnimationDuration)
            .setEase(_colorsAnimationType);
        yield return new WaitForSeconds(_colorsAnimationDuration);
        ServiceLocator.Get<IMusicService>().PlaySoundPitch("aceptar", 0.2f);

        var percentage = _percentageText.rectTransform.parent;
        var money = _moneyText.rectTransform.parent;
        var tip = _tipText.rectTransform.parent;
        var totalMoney = _totalMoneyText.rectTransform.parent;
        
        percentage.localScale = Vector3.one * _scoreAnimationInitialScale;
        money.localScale = Vector3.one * _scoreAnimationInitialScale;
        tip.localScale = Vector3.one * _scoreAnimationInitialScale;
        totalMoney.localScale = Vector3.one * _scoreAnimationInitialScale;
        
        percentage.gameObject.SetActive(true);
        LeanTween.scale(percentage.gameObject, Vector3.one, _scoreAnimationDuration)
            .setEase(_scoreAnimationType);
        yield return new WaitForSeconds(_scoreAnimationDuration);
        ServiceLocator.Get<IMusicService>().PlaySoundPitch("aceptar", 0.4f);

        if (_isEndlessGameOver)
        {
            yield return DisplayEndlessGameOver();
            yield break;
        }

        money.gameObject.SetActive(true);
        LeanTween.scale(money.gameObject, Vector3.one, _scoreAnimationDuration)
            .setEase(_scoreAnimationType);
        yield return new WaitForSeconds(_scoreAnimationDuration);
        ServiceLocator.Get<IMusicService>().PlaySoundPitch("aceptar", 0.6f);

        tip.gameObject.SetActive(true);
        LeanTween.scale(tip.gameObject, Vector3.one, _scoreAnimationDuration)
            .setEase(_scoreAnimationType);
        yield return new WaitForSeconds(_scoreAnimationDuration);
        ServiceLocator.Get<IMusicService>().PlaySoundPitch("aceptar", 0.8f);

        totalMoney.gameObject.SetActive(true);
        LeanTween.scale(totalMoney.gameObject, Vector3.one, _scoreAnimationDuration)
            .setEase(_scoreAnimationType);
        yield return new WaitForSeconds(_scoreAnimationDuration);

        ServiceLocator.Get<IMoneyService>().AddDayMoney(_score.Money + _score.Tip);
    }

    private IEnumerator DisplayEndlessGameOver()
    {
        var gameOverGO =  _gameOverText.transform.parent.gameObject;
        var highScoreGO = _highScoreText.transform.parent.gameObject;
        var newHighScoreGO = _newHighScoreText.transform.parent.gameObject;
        
        gameOverGO.transform.localScale = Vector3.one * _scoreAnimationInitialScale;
        highScoreGO.transform.localScale = Vector3.one * _scoreAnimationInitialScale;
        newHighScoreGO.transform.localScale = Vector3.one * _scoreAnimationInitialScale;
        
        gameOverGO.SetActive(true);
        LeanTween.scale(gameOverGO, Vector3.one, _scoreAnimationDuration)
            .setEase(_scoreAnimationType);
        yield return new WaitForSeconds(_scoreAnimationDuration);
        ServiceLocator.Get<IMusicService>().PlaySoundPitch("aceptar", 0.6f);
        
        var money = (int)ServiceLocator.Get<IMoneyService>().DayMoney;
        var highScore = PlayerPrefs.GetInt("HighScore", 0);
        _highScoreText.text = $"${Mathf.Max(money, highScore)}";
        highScoreGO.SetActive(true);
        LeanTween.scale(highScoreGO, Vector3.one, _scoreAnimationDuration)
            .setEase(_scoreAnimationType);
        yield return new WaitForSeconds(_scoreAnimationDuration);
        ServiceLocator.Get<IMusicService>().PlaySoundPitch("aceptar", 0.6f);


        
        if (money <= highScore) yield break;
        
        
        PlayerPrefs.SetInt("HighScore", money);

        newHighScoreGO.SetActive(true);
        LeanTween.scale(newHighScoreGO, Vector3.one, _scoreAnimationDuration)
            .setEase(_scoreAnimationType);
        yield return new WaitForSeconds(_scoreAnimationDuration);
        ServiceLocator.Get<IMusicService>().PlaySoundPitch("aceptar", 0.6f);

        LeanTween.scale(_newHighScoreText.gameObject, Vector3.one * 1.1f, _scoreAnimationDuration * .75f)
            .setLoopPingPong()
            .setEase(LeanTweenType.linear);


    }
    

    private void NextMinigame()
    {
        ServiceLocator.Get<IMusicService>().PlaySoundPitch("aceptar2");
        this.enabled = false;

        if (_isEndlessGameOver)
        {
            ServiceLocator.Get<ISceneTransitionService>().TransitionToScene("Main Menu");
        }
        else
        {
            ServiceLocator.Get<IDayService>().GoToNextMinigame();
        }
    }
}