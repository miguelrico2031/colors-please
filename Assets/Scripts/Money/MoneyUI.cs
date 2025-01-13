
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dayMoneyText;
    [SerializeField] private TextMeshProUGUI _piggyBankMoneyText;
    [SerializeField] private float _animationSpeed;
    
    private int _displayedDayMoney, _displayedPiggyBankMoney;
    private int _realDayMoney, _realPiggyBankMoney;
    private int _piggyBankGoal;
    private float _animationDelay;
    
    private IMoneyService _moneyService;
    
    private Coroutine _updateMoneyCoroutine;
    private void Start()
    {
        _animationDelay = 1f / _animationSpeed;
        _moneyService = ServiceLocator.Get<IMoneyService>();
        _piggyBankGoal = (int) _moneyService.PiggyBankGoal;
        _moneyService.OnMoneyChange += OnMoneyChange;
        UpdateMoney();
    }

    private void OnDisable()
    {
        if(_moneyService is not null)
            _moneyService.OnMoneyChange -= OnMoneyChange;
    }

    private void OnMoneyChange()
    {
        if(_updateMoneyCoroutine is not null)
            StopCoroutine(_updateMoneyCoroutine);
        
        _realDayMoney = (int) _moneyService.DayMoney;
        _realPiggyBankMoney = (int) _moneyService.PiggyBankMoney;
        _updateMoneyCoroutine = StartCoroutine(UpdateMoneyAnimation());
    }

    private IEnumerator UpdateMoneyAnimation()
    {
        bool finished = false;
        while (!finished)
        {
            if (_displayedDayMoney < _realDayMoney)
                _displayedDayMoney++;
            else if (_displayedDayMoney > _realDayMoney)
                _displayedDayMoney--;
            else
                finished = true;

            if (_displayedPiggyBankMoney < _realPiggyBankMoney)
            {
                finished = false;
                _displayedPiggyBankMoney++;
            }
            else if (_displayedPiggyBankMoney > _realPiggyBankMoney)
            {
                finished = false;
                _displayedPiggyBankMoney--;
            }
            DisplayMoney();
            if(!finished) 
                yield return new WaitForSeconds(_animationDelay);
        }
    }

    private void UpdateMoney()
    {
        _displayedDayMoney = _realDayMoney = (int) _moneyService.DayMoney;
        _displayedPiggyBankMoney = _realPiggyBankMoney = (int) _moneyService.PiggyBankMoney;
        DisplayMoney();
    }

    private void DisplayMoney()
    {
        if (_displayedDayMoney % 3 == 0)
        {
            ServiceLocator.Get<IMusicService>().PlaySoundPitch("bip");
        }
        _dayMoneyText.text = $"{_displayedDayMoney}";
        _piggyBankMoneyText.text = $"{_displayedPiggyBankMoney} / {_piggyBankGoal}";
    }

}
