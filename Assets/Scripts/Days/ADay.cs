using System.Collections.Generic;
using UnityEngine;

public abstract class ADay : ScriptableObject
{
    public abstract int MinigamesCount { get; protected set; }
    
    [field: SerializeField] public Dialogue DialogueBefore { get; private set; }
    public abstract IEnumerable<Minigame> GetMinigames();
}