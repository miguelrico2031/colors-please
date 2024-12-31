using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RGB255))]
public class RGB255Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float labelWidth = position.width * 0.4f; // Ancho del área para la etiqueta (40%)
        float fieldWidth = position.width * .171f; // Cada campo ocupa un cuarto del espacio total
        float colorBoxWidth = 20f;//position.width * 0.1f; // El cuadro de color ocupa el 10%
        float spacing = 5f; // Espaciado entre elementos

        Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);

        float rightSectionStart = position.x + position.width - (fieldWidth * 3 + colorBoxWidth + spacing * 4);
        Rect rFieldRect = new Rect(rightSectionStart, position.y, fieldWidth, position.height);
        Rect gFieldRect = new Rect(rFieldRect.xMax + spacing, position.y, fieldWidth, position.height);
        Rect bFieldRect = new Rect(gFieldRect.xMax + spacing, position.y, fieldWidth, position.height);
        Rect colorBoxRect = new Rect(bFieldRect.xMax + spacing, position.y, colorBoxWidth, position.height);


        // Dibujar etiqueta
        EditorGUI.LabelField(labelRect, label);

        SerializedProperty rProp = property.FindPropertyRelative("R");
        SerializedProperty gProp = property.FindPropertyRelative("G");
        SerializedProperty bProp = property.FindPropertyRelative("B");
        
        
        // Dibujar cuadro de color
        Color color = new Color(rProp.intValue / 255f, gProp.intValue / 255f, bProp.intValue / 255f);
        EditorGUI.DrawRect(colorBoxRect, color);

        // Dibujar campos de R, G y B
        rProp.intValue = EditorGUI.IntField(rFieldRect, rProp.intValue);
        gProp.intValue = EditorGUI.IntField(gFieldRect, gProp.intValue);
        bProp.intValue = EditorGUI.IntField(bFieldRect, bProp.intValue);

        // Clampeo de valores para que permanezcan entre 0 y 255
        rProp.intValue = Mathf.Clamp(rProp.intValue, 0, 255);
        gProp.intValue = Mathf.Clamp(gProp.intValue, 0, 255);
        bProp.intValue = Mathf.Clamp(bProp.intValue, 0, 255);

    }
}
