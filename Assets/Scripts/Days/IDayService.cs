
public interface IDayService : IService
{
    public ADay CurrentDay { get; }
    public Dialogue DialogueToDisplay { get; }
    public RGB255 TargetColor { get; }
    public RGB255 GuessedColor { get; }
    public void StartDay();
    public void GoToNextMinigame();
    public void FinishMinigame(RGB255 targetColor, RGB255 guessedColor);
}
