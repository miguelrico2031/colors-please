
public interface IDayService : IService
{
    public ADay CurrentDay { get; }
    public Dialogue DialogueToDisplay { get; }
    public bool IsEndOfDayDialogue { get; }

    public RGB255 TargetColor { get; }
    public RGB255 GuessedColor { get; }
    public void ClearNonPersistentData();
    public void StartDay();
    public void GoToNextMinigame();
    public void GoToBuckets();
    public void FinishMinigame(RGB255 targetColor, RGB255 guessedColor);

    public void Load(int dayIndex);
    public void Save(out int dayIndex);
}
