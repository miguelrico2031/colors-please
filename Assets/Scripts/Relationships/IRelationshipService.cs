using System.Collections.Generic;

public interface IRelationshipService : IService
{
    // public bool IsDirty { get; }

    public uint GetPoints(Character character);
    public void RemovePoints(Character character, uint points);
    public bool IsGameOver(out Character character);
    public void ResetPoints();

    public void Load(List<RelationshipPoints> relationshipPoints);
    public void Save(out List<RelationshipPoints> points);
    
}