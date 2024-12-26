using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DayManager")]
public class DayManager : ScriptableObject, IDayService
{
    [field: SerializeField] public ADay CurrentDay { get; private set; }
    [field: SerializeField] public Dialogue DialogueToDisplay { get; private set; }
    [field: SerializeField] public Minigame CurrentMinigame { get; private set; }

    [SerializeField] private string _chatSceneName;
    [SerializeField] private string _bucketsSceneName;

    private Queue<Minigame> _minigames;

    public void StartDay(ADay day)
    {
        CurrentDay = day;

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
}