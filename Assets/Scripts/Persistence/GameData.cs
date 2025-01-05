
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int DayIndex = 0;
    public int PiggyBankMoney = 0;
    public List<RelationshipPoints> Points;
    
    // public GameData()
    // {
    //     Points = new List<RelationshipPoints>();
    //     foreach (Character key in System.Enum.GetValues(typeof(Character)))
    //     {
    //         if (key is Character.None or Character.Yourself)
    //             continue;
    //
    //         var rp = new RelationshipPoints() { character = key, Points = 0 };
    //         Points.Add(rp);
    //     }
    // }
}
