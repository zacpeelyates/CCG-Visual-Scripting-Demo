using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TriggerRootNode))]
public class TriggerRootNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        TriggerRootNode node = (TriggerRootNode)target;
        node.PlayerFlags = (PLAYER_CONDITION_FLAGS)EditorGUILayout.EnumFlagsField("Player Flags", node.PlayerFlags);
        node.trigger = (GAME_TRIGGER)EditorGUILayout.EnumPopup("Trigger", node.trigger);
        node.TriggerOnce = EditorGUILayout.Toggle("Trigger Once", node.TriggerOnce);
        if(node.trigger.GetTriggerType() != GAME_TRIGGER_OFFSET.TurnPhase)
        {
            node.SourceFlags = (ABILITY_TARGET_FLAGS)EditorGUILayout.EnumFlagsField("Source Flags", node.SourceFlags);
            if (node.SourceFlags.HasConflicts()) node.MustMeetAllFlags = false;
            else node.MustMeetAllFlags = EditorGUILayout.Toggle("Source Must Meet All Flags", node.MustMeetAllFlags);
        }
        if(EditorGUI.EndChangeCheck())
        {
            node.OnValidate();
        }
    }
}
