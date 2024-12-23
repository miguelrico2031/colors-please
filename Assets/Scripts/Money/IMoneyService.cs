
public interface IMoneyService : IService
{
    public uint Money { get; }
    
    public void AddMoney(uint amount);

    public void RemoveMoney(uint amount);

    public void Clear();

}
