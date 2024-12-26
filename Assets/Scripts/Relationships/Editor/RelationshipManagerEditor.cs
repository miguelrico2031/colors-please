using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RelationshipManager))]
public class RelationshipManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RelationshipManager relationshipManager = (RelationshipManager)target;

        // Inicializar SerializedObject
        SerializedObject serializedObj = new SerializedObject(relationshipManager);
        SerializedProperty property = serializedObj.GetIterator();

        // Dibujar todos los campos serializables predeterminados
        property.NextVisible(true); // Saltar el campo "m_Script"
        while (property.NextVisible(false))
        {
            EditorGUILayout.PropertyField(property, true);
        }

        serializedObj.ApplyModifiedProperties();

        // Personalizar la lista de puntos
        EditorGUILayout.LabelField("Points Configuration", EditorStyles.boldLabel);

        
        foreach (Character key in System.Enum.GetValues(typeof(Character)))
        {
            if(key is Character.None or Character.Yourself)
                continue;
            
            var pair = relationshipManager.Points.Find(p => p.character == key);
            if (pair == null)
            {
                pair = new RelationshipPoints() { character = key, Points = 0 };
                relationshipManager.Points.Add(pair);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(key.ToString(), GUILayout.Width(100));
            var pointsInt = EditorGUILayout.IntField((int)pair.Points, GUILayout.Width(50));
            pair.Points = (uint)Mathf.Clamp(pointsInt, 0, relationshipManager.MaxPoints);

            EditorGUILayout.EndHorizontal();
        }
        
        if (GUILayout.Button("Reset Points"))
        {
            relationshipManager.ResetPoints();
            EditorUtility.SetDirty(target);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        
        
    }
}