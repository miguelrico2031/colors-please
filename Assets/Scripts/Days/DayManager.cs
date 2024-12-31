using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/DayManager")]
public class DayManager : ScriptableObject, IDayService
{
    [field: SerializeField] public ADay CurrentDay { get; private set; }
    [field: SerializeField] public Dialogue DialogueToDisplay { get; private set; }
    
    [field: SerializeField] public RGB255 TargetColor { get; private set; }
    [field: SerializeField] public RGB255 GuessedColor { get; private set; }
    [field: SerializeField] public Minigame CurrentMinigame { get; private set; }
    
    [SerializeField] private string _chatSceneName;
    [SerializeField] private string _bucketsSceneName;
    [SerializeField] private string _scoreSceneName;
    
    [Header("List of Days in Order")]
    [SerializeField] private List<ADay> _days;

    [SerializeField]private int _currentDayIndex = -1;
    private Queue<Minigame> _minigames;

    public void ResetDays()
    {
        _currentDayIndex = -1;
        CurrentDay = null;
        DialogueToDisplay = null;
        CurrentMinigame = null;
        _minigames = null;
    }
    public void StartDay()
    {
        _currentDayIndex++;
        if (_currentDayIndex >= _days.Count)
        {
            FinishGame();
            return;
        }
        
        CurrentDay = _days[_currentDayIndex];

        _minigames = new Queue<Minigame>(CurrentDay.GetMinigames());

        if (CurrentDay.DialogueBefore is not null)
        {
            DialogueToDisplay = CurrentDay.DialogueBefore;
            ServiceLocator.Get<ISceneTransitionService>().TransitionToScene(_chatSceneName);
        }
        else
        {
            GoToNextMinigame();
        }
    }

    public void GoToNextMinigame()
    {
        if(CurrentMinigame is null)
            TrySelectNextMinigame();
        else
            ServiceLocator.Get<ISceneTransitionService>().TransitionToScene(CurrentMinigame.SceneName);

    }

    public void FinishMinigame(RGB255 targetColor, RGB255 guessedColor)
    {
        TargetColor = targetColor;
        GuessedColor = guessedColor;
        CurrentMinigame = null;
        ServiceLocator.Get<ISceneTransitionService>().TransitionToScene(_scoreSceneName);
    }
    
    

    private void TrySelectNextMinigame()
    {
        if (_minigames.TryDequeue(out var minigame))
        {
            CurrentMinigame = minigame;
            ServiceLocator.Get<ISceneTransitionService>().TransitionToScene(CurrentMinigame.SceneName);
        }
        else
        {
            FinishDay();
        }
    }

    private void FinishDay()
    {
        ServiceLocator.Get<ISceneTransitionService>().TransitionToScene(_bucketsSceneName);
    }

    private void FinishGame()
    {
        Debug.Log("GANASTE?");
    }
}