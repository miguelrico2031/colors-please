using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/RelationshipManager")]
public class RelationshipManager : ScriptableObject, IRelationshipService
{
    [field: SerializeField] public List<RelationshipPoints> Points { get; private set; }


    // public bool IsDirty {get; private set;}

    // private Dictionary<Character, int> _dayPoints = new();

    public uint GetPoints(Character character)
    {
        return Points.First(p => p.character == character).Points;
    }

    private uint GetMaxPoints(Character character) => Points.First(p => p.character == character).MaxPoints;


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
        foreach (var rp in Points)
        {
            rp.Points = rp.MaxPoints;
        }
    }

    public bool IsGameOver(out Character character)
    {
        if (GetPoints(Character.Landlord) == 0)
        {
            character = Character.Landlord;
            return true;
        }
        else if (GetPoints(Character.Husband) == 0)
        {
            character = Character.Husband;
            return true;
        }
        else if (GetPoints(Character.Son) == 0)
        {
            character = Character.Son;
            return true;
        }
        else if (GetPoints(Character.Boss) == 0)
        {
            character = Character.Boss;
            return true;
        }

        character = Character.None;
        return false;
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