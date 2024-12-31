using System;
using TMPro;
using UnityEngine;

public class BuyableBucket : MonoBehaviour
{
    private BucketUI _bucketUI;

    private IMoneyService _moneyService;

    private void Awake()
    {
        _bucketUI = GetComponent<BucketUI>();
        _bucketUI.OnInit += OnInit;
        _bucketUI.OnSelect += OnSelect;
    }

    private void OnInit()
    {
        _bucketUI.OnInit -= OnInit;
        _moneyService = ServiceLocator.Get<IMoneyService>();
        _moneyService.OnMoneyChange += OnMoneyChange;
        OnMoneyChange();

        _bucketUI.CostText.text = $"Cost: {_bucketUI.Bucket.Cost}";
    }

    private void OnDisable()
    {
        if (_moneyService is not null)
            _moneyService.OnMoneyChange -= OnMoneyChange;
    }

    private void OnMoneyChange()
    {
        var totalMoney = _moneyService.DayMoney + _moneyService.PiggyBankMoney;
        if (totalMoney < _bucketUI.Bucket.Cost)
        {
            _bucketUI.Disable();
        }
        else
        {
            _bucketUI.Enable();
        }
    }

    private void OnSelect()
    {
        _bucketUI.OnSelect -= OnSelect;
        _moneyService.OnMoneyChange -= OnMoneyChange;
        _bucketUI.CostText.text = "Bought";
        
        RemoveMoney();
        ApplyPayedConsequences();
    }

    private void RemoveMoney()
    {
        if (_bucketUI.Bucket.Cost <= _moneyService.DayMoney)
        {
            _moneyService.RemoveDayMoney(_bucketUI.Bucket.Cost);
        }
        else
        {
            uint dayMoney = _moneyService.DayMoney;
            _moneyService.RemoveDayMoney(dayMoney);
            uint remainder = _bucketUI.Bucket.Cost - dayMoney;
            _moneyService.RemoveFromPiggyBank(remainder);
        }

    }

    private void ApplyPayedConsequences()
    {
        var relationShipService = ServiceLocator.Get<IRelationshipService>();

        foreach (var consequence in _bucketUI.Bucket.PayedConsequences)
        {
            uint points = (uint)Mathf.Abs(consequence.Points);
            if (consequence.Points > 0)
            {
                relationShipService.AddPoints(consequence.character, points);
            }
            else
            {
                relationShipService.RemovePoints(consequence.character, points);
            }
        }
    }
}