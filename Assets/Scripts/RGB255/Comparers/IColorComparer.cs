public interface IColorComparer
{
    public float MAX_DISTANCE { get; }

    public float Distance(RGB255 a, RGB255 b);
    
    public float Similarity(RGB255 a, RGB255 b);
}