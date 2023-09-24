// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEffectGraphView.cs
// Modified: 2023/05/15 @ 03:06
// Brief: Graph View Extention for Node Tree

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

#endregion

public class CardEffectGraphView : GraphView
{


    private readonly List<CardEffectNode> copiedNodes = new();

    public Action OnGraphViewChange;
    public Action<VisualNode> OnNodeSelected;


    public CardEffectGraphView()
    {
        // Import USS
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/UI/CardEditor/CardEditorWindow.uss");
        styleSheets.Add(styleSheet);

        //Add background
        GridBackground gridBackground = new();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);

        //Add Manipulators
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new SelectionDropper());
        this.AddManipulator(new RectangleSelector());

        //Setup Events 
        Undo.undoRedoPerformed += OnUndoRedo;
        serializeGraphElements += OnSerializeGraphElements;
        unserializeAndPaste += OnUnserializeAndPaste;
    }

    public CardEffectTree CurrentTree { get; private set; }

    public void UpdateNodeClasses()
    {
        foreach (VisualNode node in nodes.OfType<VisualNode>()) node.UpdateNodeStateClasses();
    }

    private void OnUnserializeAndPaste(string operationName, string data)
    {
        foreach (CardEffectNode node in copiedNodes)
        {
            CardEffectNode clone = node.ShallowClone();
            clone.GUID = GUID.Generate().ToString();
            CurrentTree.AddNodeToObject(clone);
            AddElement(new VisualNode(clone, this));
        }

        copiedNodes.Clear();
        AssetDatabase.SaveAssets();
        Populate(CurrentTree);
    }

    private string OnSerializeGraphElements(IEnumerable<GraphElement> elements)
    {
        foreach (GraphElement element in elements)
            if (element is VisualNode visualNode) copiedNodes.Add(visualNode.data);
        return string.Empty;
    }

    public void OnUndoRedo()
    {
        Populate(CurrentTree);
        AssetDatabase.SaveAssets();
    }

    public VisualNode GetVisualFromData(CardEffectNode data)
    {
        return data == null ? null : GetNodeByGuid(data.GUID) as VisualNode;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(e => e.node != startPort.node && e.direction != startPort.direction).ToList();
    }

    public void Populate(CardEffectTree effectTree)
    {
        if (effectTree == null) return;
        ClearElements();
        CurrentTree = effectTree;
        if (effectTree.nodes == null || !effectTree.nodes.Any()) return;

        //Instantiate all nodes 
        foreach (CardEffectNode node in effectTree.nodes)
        {
            VisualNode n = new(node, this);
            AddElement(n);
            n.UpdateDescription();
        }

        //Connect parents to children 
        foreach (CardEffectNode parent in effectTree.nodes)
        {
            if (parent == null) continue;
            VisualNode visualParent = GetVisualFromData(parent);
            foreach (CardEffectNode child in parent.GetChildren())
            {
                if (child == null) continue;
                child.parent = parent;
                VisualNode visualChild = GetVisualFromData(child);
                AddElement(visualParent.outputPort.ConnectTo(visualChild.inputPort));
            }
        }
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        //Remove Elements
        if (graphViewChange.elementsToRemove != null)
            foreach (GraphElement ge in graphViewChange.elementsToRemove)
                if (ge is VisualNode visualNode)
                {
                    CurrentTree.RemoveNode(visualNode.data);
                }
                else if (ge is Edge edge && edge.output.node is VisualNode parent &&
                         edge.input.node is VisualNode child)
                {
                    Undo.RecordObject(parent.data, "Remove Child");
                    parent.data.Remove(child.data);
                    child.data.parent = null;
                    EditorUtility.SetDirty(parent.data);
                }

        //Add Elements
        if (graphViewChange.edgesToCreate != null)
            foreach (Edge edge in graphViewChange.edgesToCreate)
                if (edge.output.node is VisualNode parent && edge.input.node is VisualNode child)
                {
                    Undo.RecordObject(parent.data, "Add Child");
                    parent.data.Add(child.data);
                    child.data.parent = parent.data;
                    EditorUtility.SetDirty(parent.data);
                }

        //Moved Elements
        if (graphViewChange.movedElements != null) CurrentTree.SortTreeByNodeGraphPosition();

        //Events
        OnGraphViewChange?.Invoke();
        CurrentTree.HandleKeywordData();
        return graphViewChange;
    }

    public void UpdateDescription()
    {
        nodes.Cast<VisualNode>().ToList()?.ForEach(n => n.UpdateDescription());
    }


    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (CurrentTree == null) return;
        TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<CardEffectNode>();
        foreach (Type type in types)
        {
            if (type.IsAbstract) continue; //Don't instantiate abstract types 
            evt.menu.AppendAction($"{type.BaseType?.Name}/{type.Name}", //create right click menu
                _ =>  //invoke method when item selected 
                {
                    MethodInfo method = CreateNodeMethodInfo.MakeGenericMethod(type);
                    method.Invoke(this, null);
                });
        }
    }

    //Spooky reflection method info 
    private static readonly MethodInfo CreateNodeMethodInfo =
        typeof(CardEffectGraphView).GetMethod("CreateNode", BindingFlags.NonPublic | BindingFlags.Instance);

    //method that gets invoked with the generic type of selected node
    private void CreateNode<NodeType>() where NodeType : CardEffectNode
    {
        NodeType node = CurrentTree.CreateNode<NodeType>();
        if (node == null) return;
        AddElement(new VisualNode(node, this));
    }

    private void ClearElements()
    {
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
    }

    //weird UXML requirement 
    public new class UxmlFactory : UxmlFactory<CardEffectGraphView, UxmlTraits>
    {
    }
}