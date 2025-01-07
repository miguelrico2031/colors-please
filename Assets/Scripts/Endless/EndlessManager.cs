using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EndlessManager")]

public class EndlessManager : ScriptableObject, IDayService
{
    public ADay CurrentDay => _endlessModeDay;
    public Dialogue DialogueToDisplay => null;
    public bool IsEndOfDayDialogue => false;
    public Minigame CurrentMinigame { get; private set; }
    public RGB255 TargetColor { get; private set; } 
    public RGB255 GuessedColor { get; private set; }
    public Character GameOverCharacter => Character.None;

    public int MinigamesCount { get; private set; } = 0;

    [SerializeField] private ADay _endlessModeDay;
    [SerializeField] private string _scoreSceneName;

    private Queue<Minigame> _minigames = new();

    public void StartDay()
    {
        MinigamesCount = -1;
        GoToNextMinigame();
    }

    public void GoToNextMinigame()
    {
        PlayerPrefs.SetInt("HighScore", Mathf.Max(PlayerPrefs.GetInt("HighScore", 0), (int) ServiceLocator.Get<IMoneyService>().DayMoney));
        
        MinigamesCount++;
        if(!_minigames.TryDequeue(out _))
            _minigames = new Queue<Minigame>(_endlessModeDay.GetMinigames());
        CurrentMinigame = _minigames.Dequeue();
        ServiceLocator.Get<ISceneTransitionService>().TransitionToScene(CurrentMinigame.SceneName);
    }
    

    public void FinishMinigame(RGB255 targetColor, RGB255 guessedColor)
    {
        TargetColor = targetColor;
        GuessedColor = guessedColor;
        CurrentMinigame = null;
        ServiceLocator.Get<ISceneTransitionService>().TransitionToScene(_scoreSceneName);
    }
    
    
    public void ClearNonPersistentData()
    {
        throw new Exception("No deberia esto tal");
    }
    public void GoToBuckets()
    {
        throw new Exception("No deberia esto tal");
    }

    public void EndDay()
    {
        throw new Exception("No deberia esto tal");
    }

    public void Load(int dayIndex)
    {
        throw new Exception("No deberia esto tal");
    }

    public void Save(out int dayIndex)
    {
        throw new Exception("No deberia esto tal");
    }
}
