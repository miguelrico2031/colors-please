
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/CharacterManager")]
public class CharacterManager : ScriptableObject, ICharacterService
{
    public List<CharacterInfo> Characters = new List<CharacterInfo>();

    public CharacterInfo GetCharacterInfo(Character character)
    {
        return Characters.First(c => c.Character == character);
    }
}