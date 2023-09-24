// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: GenericDictionaryPropertyDrawer.cs
// Modified: 2023/05/21 @ 20:14

#region

using UnityEditor;
using UnityEngine;

#endregion

[CustomPropertyDrawer(typeof(GenericDictionary<,>))]
public class GenericDictionaryPropertyDrawer : PropertyDrawer
{
    private const float warningBoxHeight = 1.5f;
    private static readonly float lineHeight = EditorGUIUtility.singleLineHeight;
    private static readonly float vertSpace = EditorGUIUtility.standardVerticalSpacing;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Draw list of key/value pairs.
        var list = property.FindPropertyRelative("list");
        EditorGUI.PropertyField(position, list, label, true);

        // Draw key collision warning.
        var keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            position.y += EditorGUI.GetPropertyHeight(list, true);
            if (!list.isExpanded) position.y += vertSpace;

            position.height = lineHeight * warningBoxHeight;
            position = EditorGUI.IndentedRect(position);
            EditorGUI.HelpBox(position, "Duplicate keys will not be serialized.", MessageType.Warning);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        var list = property.FindPropertyRelative("list");
        height += EditorGUI.GetPropertyHeight(list, true);

        bool keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
        if (keyCollision)
        {
            height += warningBoxHeight * lineHeight;
            if (!list.isExpanded) height += vertSpace;
        }

        return height;
    }
}