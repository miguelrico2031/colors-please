
public interface IRelationshipService : IService
{
    public uint MaxPoints { get; } //los puntos de cada relacion van de 0 a MaxPoints y comienzan en MaxPoints / 2
    public uint GetPoints(Character character);
    public void AddPoints(Character character, uint points);
    public void RemovePoints(Character character, uint points);
}
