using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RelationshipManager")]
public class RelationshipManager : ScriptableObject, IRelationshipService
{
    [field: SerializeField] public uint MaxPoints{ get; private set; }

    public List<RelationshipPoints> Points { get; private set; } = new();
    

    public uint GetPoints(Character character)
    {
        return Points.First(p => p.character == character).Points;
    }

    public void AddPoints(Character character, uint points)
    {
        var relationshipPoints = Points.First(p => p.character == character);
        
        var newPoints = relationshipPoints.Points + points;
       relationshipPoints.Points = (uint) Mathf.Clamp(newPoints, 0, MaxPoints);
    }

    public void RemovePoints(Character character, uint points)
    {
        var relationshipPoints = Points.First(p => p.character == character);
            
        var newPoints = points < relationshipPoints.Points
            ? relationshipPoints.Points - points
            : 0;
        relationshipPoints.Points = newPoints;
    }
    
    public void ResetPoints()
    {
        uint newValue = MaxPoints / 2;
        foreach (var rp in Points)
        {
            rp.Points = newValue;
        }
    }
}