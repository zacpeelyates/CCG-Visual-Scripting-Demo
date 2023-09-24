// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEffectTree.cs
// Modified: 2023/05/21 @ 19:22

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

#endregion

[CreateAssetMenu(fileName = "new CardEffectTree", menuName = "Tree/CardEffect", order = 0)]
public class CardEffectTree : ScriptableObject
{
    [NonSerialized] private readonly HashSet<CardEffectRootNode> KeywordRoots = new();

    [NonSerialized] private readonly List<CardEffectTree> KeywordTrees = new();
    [NonSerialized] public HashSet<CardEffectNode> nodes = new();
    [NonSerialized] private CardData owner;
    [NonSerialized] protected HashSet<CardEffectRootNode> rootsInternal = new();
    private HashSet<KeywordNode> KeywordNodes => nodes.OfType<KeywordNode>().ToHashSet();

    public virtual List<CardEffectRootNode> GetRoots()
    {
        if (KeywordRoots == null) return rootsInternal.ToList();
        return rootsInternal.Concat(KeywordRoots).ToList();
    }

    public void LoadFromScriptableObject() //load tree in this asset
    {
        nodes.Clear();
        rootsInternal.Clear();
        object[] children = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
        foreach (var child in children)
            if (child is CardEffectNode node)
            {
                nodes.Add(node);
                if (node is not CardEffectRootNode root) continue;
                root.SetOwner(this);
                rootsInternal.Add(root);
            }

        HandleKeywordData();
    }

    public void SetupFields() //setup callbacks in nodes and keyword nodes 
    {
        nodes.OfType<PerformGameActionNode>().ToList().ForEach(x => x.SetupCallback());
        foreach (var kwRoot in KeywordRoots)
            kwRoot.GetChildrenWhere(x => x is PerformGameActionNode).ToList()
                .ForEach(x => (x as PerformGameActionNode)?.SetupCallback());
    }

    public CardData GetOwner()
    {
        return owner;
    }

    public void SetOwner(CardData dataParent)
    {
        owner = dataParent;
    }

    public void HandleKeywordData() //load keyword effects into this tree
    {
        foreach (var keyword in KeywordNodes)
        {
            if (keyword == null) continue;
            CardEffectTree KeywordTree = keyword.GetKeywordTree();
            if (KeywordTree == null) continue;
            if (!KeywordTrees.Contains(KeywordTree)) KeywordTrees.Add(KeywordTree);

            KeywordTree.LoadFromScriptableObject();

            foreach (var root in KeywordTree.GetRoots())
            {
                root.SetOwner(this);
                KeywordRoots.Add(root); //hashset wont allow duplicates 
            }
        }
    }

    public virtual NodeType CreateNode<NodeType>() where NodeType : CardEffectNode
    {
        var node = CreateInstance<NodeType>();
        AddNodeToObject(node);
        return node;
    }

    public void ClearOrphans()
    {
        List<CardEffectNode> connectedNodes = new();
        rootsInternal?.ToList().ForEach(r => ApplyFunctionRecursive(r, n => connectedNodes.Add(n)));
        nodes?.Where(n => !connectedNodes.Contains(n)).ToList().ForEach(n =>
        {
            if (n is CardEffectStaticNode s) return;
            nodes.Remove(n);
            Undo.DestroyObjectImmediate(n);
        });
    }

    public void AddNodeToObject<NodeType>(NodeType node) where NodeType : CardEffectNode
    {
        if (node is CardEffectRootNode root && HandleNewRoot(root) == false) return;
        node.name = typeof(NodeType).Name;
        node.GUID = Guid.NewGuid().ToString();
        Undo.RecordObject(this, "Create Node");
        nodes.Add(node);
        AssetDatabase.AddObjectToAsset(node, this);
        Undo.RegisterCreatedObjectUndo(node, "Create Node");
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    protected virtual bool HandleNewRoot(CardEffectRootNode root)
    {
        rootsInternal ??= new HashSet<CardEffectRootNode>();
        rootsInternal.Add(root);
        return true;
    }

    public void RemoveNode(CardEffectNode node)
    {
        if (node is CardEffectRootNode root && rootsInternal.Contains(root)) rootsInternal.Remove(root);
        Undo.RecordObject(this, "Remove Node (tree)");
        nodes.Remove(node);
        Undo.DestroyObjectImmediate(node);
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public void RemoveNullNodes()
    {
        nodes?.Remove(null);
        rootsInternal?.Remove(null);
    }

    public CardEffectTree Clone(CardData parent)
    {
        var CloneTree = Instantiate(this);
        CloneTree.SetOwner(parent);
        CloneTree.rootsInternal = new HashSet<CardEffectRootNode>();
        CloneTree.nodes = new HashSet<CardEffectNode>();
        foreach (var root in rootsInternal)
        {
            var cloneRoot = (CardEffectRootNode)root.DeepClone(null);
            CloneTree.rootsInternal.Add(cloneRoot);
            cloneRoot.SetOwner(CloneTree);
        }

        foreach (var keyword in KeywordNodes)
        {
            var cloneKeyword = (KeywordNode)keyword.DeepClone(null);
            CloneTree.nodes.Add(cloneKeyword);
        }

        CloneTree.HandleKeywordData();
        CloneTree.AddRecursive(CloneTree.rootsInternal.Cast<CardEffectNode>().ToList());
        CloneTree.SetupFields();
        return CloneTree;
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID)
    {
        var obj = Selection.activeObject;
        CardEffectTree tree = null;
        if (obj is CardEffectTree t) tree = t;
        else if (obj is CardData c) tree = c.GetEffectTree();
        if (tree == null) return false;
        tree.LoadFromScriptableObject();
        CardEditorWindow.Open();
        return true;
    }

    public static void ApplyFunctionRecursive(CardEffectNode node, Action<CardEffectNode> func)
    {
        if (node == null) return;
        func.Invoke(node);
        node.GetChildren().ForEach(n => ApplyFunctionRecursive(n, func));
    }

    public string Parse(bool recursive) //parses tree 
    {
        if (nodes == null || !nodes.Any()) return string.Empty;
        StringBuilder stringBuilder = new();
        var i = 0;
        foreach (var node in KeywordNodes)
        {
            stringBuilder.Append(node.GetDescriptionString(false));
            stringBuilder.Append(i++ == KeywordNodes.Count - 1 ? '.' : ',');
        }

        rootsInternal?.ToList().ForEach(r => stringBuilder.Append(r.Parse(recursive) + '\n'));
        return stringBuilder.ToString();
    }

    public void AddRecursive(List<CardEffectNode> newNodes) //adds list of nodes including children
    {
        if (newNodes == null || newNodes.Count == 0) return;
        foreach (var n in newNodes)
        {
            nodes.Add(n);
            AddRecursive(n.GetChildren());
        }
    }

    public void SortTreeByNodeGraphPosition() //allows effect execution to be reordered simply moving the elements rather than having to remove and readd connections
    {
        rootsInternal?.ToList()
            .ForEach(root => ApplyFunctionRecursive(root, n => n?.SortChildrenByNodeGraphPosition()));
    }
}