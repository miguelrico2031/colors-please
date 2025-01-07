
using System;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private GameObject _endlessButton;
    [SerializeField] private GameObject _endlessTutorial;
    [SerializeField] private TextMeshProUGUI _endlessInfoText;
    [SerializeField] private TextMeshProUGUI _endlessTutorialText;
    [SerializeField] private TextMeshProUGUI _saveInfoText;
    [SerializeField] private GameObject _title;
    [SerializeField] private float _minTitleScale;
    [SerializeField] private float _titleScalePeriod;
    [SerializeField] private LeanTweenType _titleScaleEaseType;
    [SerializeField] private float _minTitleRotation = 45f;
    [SerializeField] private float _titleRotationPeriod = 1.090f;
    [SerializeField] private LeanTweenType _titleRoationEaseType;

    private bool _showEndlessTutorial;
    
    private void Start()
    {
        Initializer.Instance.SetDayMode();

        // PlayerPrefs.SetInt("Endless", 1);
        if (PlayerPrefs.GetInt("Endless", 0) == 0)
        {
            _endlessButton.SetActive(false);
        }
        else
        {
            var highScore = PlayerPrefs.GetInt("HighScore", 0);
            if (highScore > 0)
            {
                _endlessInfoText.text = $"Mejor partida: ${highScore}";
            }
            else
            {
                _showEndlessTutorial = true;
            }
        }
        
        if (!ServiceLocator.Get<IPersistenceService>().TryGetSavedData(out var data))
        {
            _continueButton.SetActive(false);
            return;
        }
        
        _saveInfoText.text = $"Día {data.DayIndex + 1}\nHucha: ${data.PiggyBankMoney}";

        ServiceLocator.Get<IMusicService>().SetSong("clips");
        ServiceLocator.Get<IMusicService>().SetPhase(2);

        LeanTween.scale(_title, Vector3.one * _minTitleScale, _titleScalePeriod)
            .setLoopPingPong()
            .setEase(_titleScaleEaseType);
        
        LeanTween.rotateAroundLocal(_title, Vector3.forward, _minTitleRotation, _titleRotationPeriod)
            .setLoopPingPong()
            .setEase(_titleRoationEaseType);
    }
    
    public void NewGame()
    {
        ServiceLocator.Get<IPersistenceService>().NewSave();
        ServiceLocator.Get<IDayService>().ClearNonPersistentData();
        ServiceLocator.Get<IDayService>().StartDay();
        if (ServiceLocator.Get<IDayService>().CurrentDay.DialogueBefore == null)
        {
            ServiceLocator.Get<IMusicService>().SetPhase(1);
        }
    }

    public void ContinueGame()
    {
        LoadGame();
    }   

    public static void LoadGame()
    {
        ServiceLocator.Get<IPersistenceService>().Load();
        ServiceLocator.Get<IDayService>().ClearNonPersistentData();
        ServiceLocator.Get<IDayService>().StartDay();
        if (ServiceLocator.Get<IDayService>().CurrentDay.DialogueBefore == null)
        {
            ServiceLocator.Get<IMusicService>().SetPhase(1);
        }
    }

    public void StartEndless()
    {
        if (_showEndlessTutorial)
        {
            _showEndlessTutorial = false;
            _endlessTutorial.SetActive(true);
            float minPercentage = ServiceLocator.Get<IScoreService>().EndlessMinPercentage;
            int minPercentageInt = Mathf.RoundToInt(minPercentage);
            _endlessTutorialText.text = $"¡Has desbloqueado el Modo Infinito!\n" +
                                        $"Juega sin parar, pero cuidado: si sacas menos de {minPercentageInt}%" +
                                        $" de similitud, ¡pierdes!\n" +
                                        $"Desafía tu mejor puntuación y llega más lejos cada vez.\n";
            return;
        }
        
        Initializer.Instance.SetEndlessMode();
        ServiceLocator.Get<IMoneyService>().Load(0);
        ServiceLocator.Get<IDayService>().StartDay();
        ServiceLocator.Get<IMusicService>().SetPhase(1);
    }
    
    public void CloseEndlessTutorial() => _endlessTutorial.SetActive(false);
}
