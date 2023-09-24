// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEditor.cs
// Modified: 2023/04/25 @ 01:51

#region

using UnityEditor;
using UnityEngine;

#endregion

[CustomEditor(typeof(Card))]
public class CardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Card cardData = (Card)target;
        if (GUILayout.Button("Load")) cardData.LoadCardDataFromInspector(); 
    }
}