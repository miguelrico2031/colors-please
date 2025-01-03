
using System;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private TextMeshProUGUI _saveInfoText;

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
