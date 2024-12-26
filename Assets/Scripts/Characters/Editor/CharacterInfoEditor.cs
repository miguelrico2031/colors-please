
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterManager))]

public class CharacterInfoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CharacterManager characterManager = (CharacterManager)target;

        foreach (Character key in System.Enum.GetValues(typeof(Character)))
        {
            if(key is Character.None) continue;
            
            var characterInfo = characterManager.Characters.Find(p => p.Character == key);
            if (characterInfo == null)
            {
                characterInfo = new CharacterInfo() { Character = key };
                characterManager.Characters.Add(characterInfo);
            }

            EditorGUILayout.LabelField(key.ToString(), EditorStyles.boldLabel);
            characterInfo.Name = EditorGUILayout.TextField("Name", characterInfo.Name);
            characterInfo.ChatColor = EditorGUILayout.ColorField("Chat Color", characterInfo.ChatColor);
            characterInfo.Picture = (Sprite)EditorGUILayout.ObjectField("Picture", characterInfo.Picture, typeof(Sprite), false);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
