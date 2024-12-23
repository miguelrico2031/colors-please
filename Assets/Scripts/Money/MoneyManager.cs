using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MoneyManager")]
public class MoneyManager : ScriptableObject, IMoneyService
{
    [field: SerializeField] public uint Money { get; private set; }
    
    
    public void AddMoney(uint amount)
    {
        Money += amount;
    }


    public void RemoveMoney(uint amount)
    {
        if (amount > Money)
            throw new Exception("Tried to remove more money than possible.");

        Money -= amount;
    }

    public void Clear()
    {
        Money = 0;
    }
}
