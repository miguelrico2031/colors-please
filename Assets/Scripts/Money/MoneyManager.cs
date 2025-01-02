using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MoneyManager")]
public class MoneyManager : ScriptableObject, IMoneyService
{
    [field: SerializeField] public uint DayMoney { get; private set; }
    [field: SerializeField] public uint PiggyBankMoney { get; private set; }
    [field: SerializeField] public uint PiggyBankGoal { get; private set; }

    // public bool IsDirty { get; private set; }
    public event Action OnMoneyChange;
    
    // private uint _cleanPiggyBankMoney;

    
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

        // if (!IsDirty)
        // {
        //     IsDirty = true;
        //     _cleanPiggyBankMoney = PiggyBankMoney;
        // }
        PiggyBankMoney += DayMoney;
        DayMoney = 0;
        OnMoneyChange?.Invoke();
        return true;
    }

    public bool RemoveFromPiggyBank(uint amount)
    {
        if (amount > PiggyBankMoney)
            return false;
        
        // if (!IsDirty)
        // {
        //     IsDirty = true;
        //     _cleanPiggyBankMoney = PiggyBankMoney;
        // }
        PiggyBankMoney -= amount;
        OnMoneyChange?.Invoke();
        return true;
    }


    public void Load(int piggyBankMoney)
    {
        PiggyBankMoney = (uint) piggyBankMoney;
    }

    public void Save(out int piggyBankMoney)
    {
        // ApplyChanges();
        // piggyBankMoney = _cleanPiggyBankMoney;
        piggyBankMoney = (int) PiggyBankMoney;
    }

    // private void ApplyChanges()
    // {
    //     _cleanPiggyBankMoney = PiggyBankMoney;
    //     IsDirty = false;
    // }

}
