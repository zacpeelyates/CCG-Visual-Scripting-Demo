// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: NodeInspector.cs
// Modified: 2023/05/13 @ 22:29

#region

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#endregion

public class NodeInspector : VisualElement
{
    private Editor current;

    internal void UpdateSelection(VisualNode node)
    {
        Clear();
        Object.DestroyImmediate(current);
        current = Editor.CreateEditor(node.data);
        if (current != null) Add(new IMGUIContainer(() => current.OnInspectorGUI()));
    }

    public new class UxmlFactory : UxmlFactory<NodeInspector, UxmlTraits>
    {
    }
}