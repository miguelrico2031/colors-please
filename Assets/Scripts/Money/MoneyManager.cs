using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MoneyManager")]
public class MoneyManager : ScriptableObject, IMoneyService
{
    [field: SerializeField] public uint DayMoney { get; private set; }
    [field: SerializeField] public uint PiggyBankMoney { get; private set; }
    [field: SerializeField] public uint PiggyBankGoal { get; private set; }

    public event Action OnMoneyChange;

    
    public void AddDayMoney(uint amount)
    {
        DayMoney += amount;
        OnMoneyChange?.Invoke();
    }


    public void RemoveDayMoney(uint amount)
    {
        if (amount > DayMoney)
            throw new Exception("Tried to remove more money than possible.");

        DayMoney -= amount;
        OnMoneyChange?.Invoke();
    }

    public bool PutInPiggyBank()
    {
        if (DayMoney == 0) 
            return false;
        PiggyBankMoney += DayMoney;
        DayMoney = 0;
        OnMoneyChange?.Invoke();
        return true;
    }

    public bool RemoveFromPiggyBank(uint amount)
    {
        if (amount > PiggyBankMoney)
            return false;
        
        PiggyBankMoney -= amount;
        OnMoneyChange?.Invoke();
        return true;
    }

}
