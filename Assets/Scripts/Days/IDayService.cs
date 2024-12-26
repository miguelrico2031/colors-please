
public interface IDayService : IService
{
    public ADay CurrentDay { get; }
    public Dialogue DialogueToDisplay { get; }
    public void StartDay(ADay day);
    public void GoToNextMinigame();
}
