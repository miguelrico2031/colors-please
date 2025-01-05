
using System;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private TextMeshProUGUI _saveInfoText;
    [SerializeField] private GameObject _title;
    [SerializeField] private float _minTitleScale;
    [SerializeField] private float _titleScalePeriod;
    [SerializeField] private LeanTweenType _titleScaleEaseType;
    [SerializeField] private float _minTitleRotation;
    [SerializeField] private float _titleRotationPeriod;
    [SerializeField] private LeanTweenType _titleRoationEaseType;
    
    private void Start()
    {
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
        ServiceLocator.Get<IPersistenceService>().Load();
        ServiceLocator.Get<IDayService>().ClearNonPersistentData();
        ServiceLocator.Get<IDayService>().StartDay();
        if (ServiceLocator.Get<IDayService>().CurrentDay.DialogueBefore == null)
        {
            ServiceLocator.Get<IMusicService>().SetPhase(1);
        }
    }
}
