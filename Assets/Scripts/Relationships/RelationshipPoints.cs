using UnityEngine.Serialization;

[System.Serializable]
public class RelationshipPoints
{
    [FormerlySerializedAs("Relationship")] public Character character;
    public uint Points;
}