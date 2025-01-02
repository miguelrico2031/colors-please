using System.Collections.Generic;

public interface IRelationshipService : IService
{
    public uint MaxPoints { get; } //los puntos de cada relacion van de 0 a MaxPoints y comienzan en MaxPoints / 2
    // public bool IsDirty { get; }

    public uint GetPoints(Character character);
    public void AddPoints(Character character, uint points);
    public void RemovePoints(Character character, uint points);

    public void Load(List<RelationshipPoints> relationshipPoints);
    public void Save(out List<RelationshipPoints> points);
}