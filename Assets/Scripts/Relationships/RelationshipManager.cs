using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RelationshipManager")]
public class RelationshipManager : ScriptableObject, IRelationshipService
{
    [field: SerializeField] public uint MaxPoints { get; private set; }

    public List<RelationshipPoints> Points { get; private set; } = new();
    
    // public bool IsDirty {get; private set;}

    // private Dictionary<Character, int> _dayPoints = new();

    public uint GetPoints(Character character)
    {
        throw new NotImplementedException();
        return Points.First(p => p.character == character).Points;
    }

    public void AddPoints(Character character, uint points)
    {
        // IsDirty = true;
        // _dayPoints.TryAdd(character, 0);
        // _dayPoints[character] += (int)points;
         
         var relationshipPoints = Points.First(p => p.character == character);
         
         var newPoints = relationshipPoints.Points + points;
        relationshipPoints.Points = (uint) Mathf.Clamp(newPoints, 0, MaxPoints);
    }

    public void RemovePoints(Character character, uint points)
    {
        // IsDirty = true;
        // _dayPoints.TryAdd(character, 0);
        // _dayPoints[character] -= (int)points;

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


    public void Load(List<RelationshipPoints> relationshipPoints)
    {
        Points = relationshipPoints;
    }

    public void Save(out List<RelationshipPoints> points)
    {
        // ApplyChanges();
        points = Points;
    }

    // private void ApplyChanges()
    // {
    //     foreach (var rp in Points)
    //     {
    //         if (!_dayPoints.TryGetValue(rp.character, out var dayPoints)) continue;
    //         rp.Points = dayPoints > 0
    //             ? rp.Points + (uint)Mathf.Abs(dayPoints)
    //             : rp.Points - (uint)Mathf.Abs(dayPoints);
    //     }
    //     _dayPoints.Clear();
    //     IsDirty = false;
    // }
}