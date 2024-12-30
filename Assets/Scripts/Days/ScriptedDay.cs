
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Day/ScriptedDay")]
public class ScriptedDay : ADay
{
    [SerializeField] private List<Minigame> _minigames;
    
    
    public override int MinigamesCount
    {
        get => _minigames is null ? 0 : _minigames.Count;
        protected set {}
    }
    public override IEnumerable<Minigame> GetMinigames() => _minigames;
}
