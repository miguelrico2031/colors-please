
public interface IScoreService : IService
{
    public float EndlessMinPercentage { get; }
    public Score GetScore(RGB255 targetColor, RGB255 guessedColor);
}