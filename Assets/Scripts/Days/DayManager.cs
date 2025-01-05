using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/DayManager")]
public class DayManager : ScriptableObject, IDayService
{
    [field: SerializeField] public ADay CurrentDay { get; private set; }
    [field: SerializeField] public Dialogue DialogueToDisplay { get; private set; }
    public bool IsEndOfDayDialogue { get; private set; }
    
    [field: SerializeField] public RGB255 TargetColor { get; private set; }
    [field: SerializeField] public RGB255 GuessedColor { get; private set; }
    [field: SerializeField] public Minigame CurrentMinigame { get; private set; }
    [field: SerializeField] public Character GameOverCharacter { get; private set; }
    
    [SerializeField] private string _chatSceneName;
    [SerializeField] private string _bucketsSceneName;
    [SerializeField] private string _scoreSceneName;
    
    [Header("List of Days in Order")]
    [SerializeField] private List<ADay> _days;

    [SerializeField]private int _currentDayIndex = -1;
    private Queue<Minigame> _minigames;

    public void ClearNonPersistentData()
    {
        // _currentDayIndex = -1;
        CurrentDay = null;
        DialogueToDisplay = null;
        CurrentMinigame = null;
        _minigames = null;
    }
    public void StartDay()
    {
        _currentDayIndex++;
        ServiceLocator.Get<IPersistenceService>().Save();
        
        int index = Mathf.Min(_currentDayIndex, _days.Count - 1);
        
        CurrentDay = _days[index];

        _minigames = new Queue<Minigame>(CurrentDay.GetMinigames());

        if (CurrentDay.DialogueBefore is not null)
        {
            DialogueToDisplay = CurrentDay.DialogueBefore;
            IsEndOfDayDialogue = false;
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
    
    public void GoToBuckets()
    {
        ServiceLocator.Get<ISceneTransitionService>().TransitionToScene(_bucketsSceneName);
    }

    public void EndDay()
    {
        //comprobar condicion de victoria
        var moneyService = ServiceLocator.Get<IMoneyService>();
        if (moneyService.PiggyBankMoney >= moneyService.PiggyBankGoal)
        {
            Win();
        }
        //comprobar condiciones de derrota
        else if (ServiceLocator.Get<IRelationshipService>().IsGameOver(out var character))
        {
            GameOverCharacter = character;
            Lose();
        }
        else
        {
            StartDay();
        }
    }

    public void Load(int dayIndex)
    {
        _currentDayIndex = dayIndex - 1; //porque se va a ++ al principio de StartDay
    }

    public void Save(out int dayIndex)
    {
        dayIndex = _currentDayIndex;   
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
        if (CurrentDay.DialogueAfter is not null)
        {
            DialogueToDisplay = CurrentDay.DialogueAfter;
            IsEndOfDayDialogue = true;
            ServiceLocator.Get<ISceneTransitionService>().TransitionToScene(_chatSceneName);
        }
        else GoToBuckets();
    }

    private void Win()
    {
        ServiceLocator.Get<ISceneTransitionService>().TransitionToScene("Win");
    }

    private void Lose()
    {
        ServiceLocator.Get<ISceneTransitionService>().TransitionToScene("Lose");
    }


    private void FinishGame()
    {
        Debug.Log("GANASTE?");
    }
}