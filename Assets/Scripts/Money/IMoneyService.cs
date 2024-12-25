
using System;

public interface IMoneyService : IService
{
    public uint DayMoney { get; }
    public uint PiggyBankMoney { get; }
    
    public uint PiggyBankGoal { get; }

    public event Action OnMoneyChange;
    
    public void AddDayMoney(uint amount);

    public void RemoveDayMoney(uint amount);

    public bool PutInPiggyBank();

    public bool RemoveFromPiggyBank(uint amount);

}
