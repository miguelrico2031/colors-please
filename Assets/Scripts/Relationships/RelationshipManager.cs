using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RelationshipManager")]
public class RelationshipManager : ScriptableObject, IRelationshipService
{
    [field: SerializeField] public uint MaxPoints{ get; private set; }

    public List<RelationshipPoints> Points { get; private set; } = new();
    

    public uint GetPoints(Relationships relationship)
    {
        return Points.First(p => p.Relationship == relationship).Points;
    }

    public void AddPoints(Relationships relationship, uint points)
    {
        var relationshipPoints = Points.First(p => p.Relationship == relationship);
        
        var newPoints = relationshipPoints.Points + points;
       relationshipPoints.Points = (uint) Mathf.Clamp(newPoints, 0, MaxPoints);
    }

    public void RemovePoints(Relationships relationship, uint points)
    {
        var relationshipPoints = Points.First(p => p.Relationship == relationship);
            
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