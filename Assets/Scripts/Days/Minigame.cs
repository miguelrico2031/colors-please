using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Minigame")]
public class Minigame : ScriptableObject
{
    public string SceneName;
    [TextArea(5, 20)] public string HelpText;
}