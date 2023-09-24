// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: SummonNodeEditor.cs
// Modified: 2023/05/21 @ 19:22

#region

using UnityEditor;

#endregion

[CustomEditor(typeof(SummonNode))]
public class SummonNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SummonNode node = (SummonNode)target;
        EditorGUI.BeginChangeCheck();
        node.cardToSummon = (CardData)EditorGUILayout.ObjectField("Card to Summon", node.cardToSummon, typeof(CardData), false);
        node.Amount = EditorGUILayout.IntField("Amount", node.Amount);
        if (EditorGUI.EndChangeCheck())
        {
            node.OnValidate();
        }
    }
}