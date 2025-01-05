using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PersistenceManager")]
public class PersistenceManager : ScriptableObject, IPersistenceService
{
    [SerializeField] private string _saveKey;



    public void Save()
    {
        GameData data = new();

        ServiceLocator.Get<IDayService>().Save(out data.DayIndex);
        ServiceLocator.Get<IMoneyService>().Save(out data.PiggyBankMoney);
        ServiceLocator.Get<IRelationshipService>().Save(out data.Points);

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(_saveKey, json);
        PlayerPrefs.Save();
        Debug.Log("Game saved!");
    }

    public void NewSave()
    {
        GameData data = new();

        
        ServiceLocator.Get<IDayService>().Load(data.DayIndex);
        ServiceLocator.Get<IMoneyService>().Load(data.PiggyBankMoney);
        // ServiceLocator.Get<IRelationshipService>().Load(data.Points);
        ServiceLocator.Get<IRelationshipService>().ResetPoints();
        ServiceLocator.Get<IRelationshipService>().Save(out data.Points);

        
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(_saveKey, json);
        PlayerPrefs.Save();
        
        Debug.Log("New game saved!");
    }

    public void Load()
    {
        if (!TryGetSavedData(out var data))
            throw new Exception("No saved data found!");
        
        ServiceLocator.Get<IDayService>().Load(data.DayIndex);
        ServiceLocator.Get<IMoneyService>().Load(data.PiggyBankMoney);
        ServiceLocator.Get<IRelationshipService>().Load(data.Points);
        Debug.Log("Game loaded!");

    }

    public bool TryGetSavedData(out GameData data)
    {
        if (PlayerPrefs.HasKey(_saveKey))
        {
            string json = PlayerPrefs.GetString(_saveKey);
            data = JsonUtility.FromJson<GameData>(json);
            return true;
        }
        else
        {
            data = null;
            return false;
        }
    }

}