
public interface IScoreService : IService
{
    public Score GetScore(RGB255 targetColor, RGB255 guessedColor);
}