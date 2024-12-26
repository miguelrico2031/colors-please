using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ScriptableObjects/Bucket")]
public class Bucket : ScriptableObject
{
    [SerializeField] [TextArea(minLines: 5, maxLines: 10)]
    public string Description;
    [field: SerializeField] public uint Cost { get; private set; }

    [Header("Use negative numbers to make consequences\nthat decrease relationship points")] 
    [SerializeField] public List<Consequence> PayedConsequences = new();
    [SerializeField] public List<Consequence> NotPayedConsequences = new();


    [Serializable]
    public class Consequence
    {
        [FormerlySerializedAs("Relationship")] public Character character;
        public int Points;
    }
}
