// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: PerformGameActionNodeEditor.cs
// Modified: 2023/05/21 @ 19:22
// Brief: Editor for PerformGameAction nodes for assigning values/callbacks
// (originally was done with property drawers but it made me want to tear my hair out) 

#region

using UnityEditor;
using UnityEngine;

#endregion

[CustomEditor(typeof(PerformGameActionNode), true)]
public class PerformGameActionNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        PerformGameActionNode node = (PerformGameActionNode)target;
        node.args.amount.useCallback = EditorGUILayout.Toggle("Amount Callback", node.args.amount.useCallback);
        EditorGUILayout.BeginHorizontal();
        if (!node.args.amount.useCallback) node.args.amount.Value = EditorGUILayout.IntField(node.args.amount.ValueBackingField);
        EditorGUILayout.EndHorizontal();

        node.args.targets.useCallback = EditorGUILayout.Toggle("Target Callback", node.args.targets.useCallback);
        if (!node.args.targets.useCallback)
        {
            node.numberOfTargets = EditorGUILayout.IntField("Number of Targets", node.numberOfTargets);
            node.LegalTargets = (ABILITY_TARGET_FLAGS)EditorGUILayout.EnumFlagsField("Legal Targets", node.LegalTargets);
            if (node.LegalTargets.HasConflicts())
            {
                node.mustMeetAllFlags = false;
                GUI.enabled = false;
            }

           if(node is not DrawCardNode) node.mustMeetAllFlags = EditorGUILayout.Toggle("Target Must Meet All Flags?", node.mustMeetAllFlags);
        }
        else
        {
            node.targetCallbackType = (PerformGameActionNode.TargetCallbackType)EditorGUILayout.EnumPopup(node.targetCallbackType);
        }

        //not sure why this isnt being called for child classes but i need to submit this and sleep lol
        if (node is DrawCardNode)
        {
            if (node.numberOfTargets > 2) node.numberOfTargets = 2;
            if (!node.LegalTargets.Contains(ABILITY_TARGET_FLAGS.Player)) node.LegalTargets |= ABILITY_TARGET_FLAGS.Player;
            if (node.LegalTargets.Contains(ABILITY_TARGET_FLAGS.Creature)) node.LegalTargets &= ~ABILITY_TARGET_FLAGS.Creature;
            if (node.numberOfTargets != 1) node.LegalTargets &= ~(ABILITY_TARGET_FLAGS.Ally | ABILITY_TARGET_FLAGS.Enemy);

        }
        if(EditorGUI.EndChangeCheck())
        {
            node.OnValidate();
        }
    }
}