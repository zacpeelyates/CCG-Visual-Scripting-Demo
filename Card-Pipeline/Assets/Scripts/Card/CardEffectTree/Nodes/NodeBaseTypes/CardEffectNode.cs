// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEffectNode.cs
// Modified: 2023/05/15 @ 03:06
// Breif: Abstract Node ScriptableObject 

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

#endregion

public abstract class CardEffectNode : ScriptableObject
{
    public enum NodeState
    {
        Running,
        Success,
        Failure,
        Idle
    }

    public enum PortType
    {
        None,
        Single,
        Multi
    }

    [HideInInspector] public Vector2 CachedGraphPosition; //used for editor
    [HideInInspector] public string description; //is generated and displayed in tool
    [HideInInspector] public string GUID;

    [HideInInspector] public CardEffectNode parent;

    protected CardEffectNode()
    {
        if (GUID is null or "") GUID = Guid.NewGuid().ToString();
    }

    public NodeState State { get; private set; } = NodeState.Idle;

    public bool ContainsRecursive(CardEffectNode node)
    {
        List<CardEffectNode> children = GetChildren();
        if (children == null || children.Count == 0) return false;
        if (children.Contains(node)) return true;
        foreach (CardEffectNode child in children)
            if (child.ContainsRecursive(node))
                return true;
        return false;
    }

    public abstract PortType GetInputPortType();
    public abstract PortType GetOutputPortType();

    public NodeState Run()
    {
        State = OnStart();
        while (State == NodeState.Running) Tick();
        return State;
    }
    public string GetDescriptionString(bool recursive)
    {
        return Regex.Replace(GetDescriptionStringInternal(recursive), @"[ ]+", " ");
    }
    protected virtual string GetDescriptionStringInternal(bool recursive)
    {
        return string.Empty;
    }
    public virtual string Parse(bool recursive) //used to generate description
    {
        string s = GetDescriptionString(recursive);
        if (!recursive) return s;
        StringBuilder stringBuilder = new(s);
        if (s == string.Empty) GetChildren().ForEach(x => stringBuilder.Append(x.Parse(true)));
        return stringBuilder.ToString();
    }


    public void OnValidate()
    {
        description = GetDescriptionString(false);
        GetChildren().RemoveAll(x => x == null);
        CardEditorWindow.instance?.UpdateDescriptionText();
    }

    public void Reset()
    {
        State = NodeState.Idle;
    }

    public List<CardEffectNode> GetChildrenWhere(Func<CardEffectNode, bool> func)
    {
        List<CardEffectNode> result = new();
        foreach (CardEffectNode child in GetChildren())
        {
            if (func(child)) result.Add(child);
            result.AddRange(child.GetChildrenWhere(func));
        }

        return result;
    }

    public void Tick() //Run node
    {
        if (State == NodeState.Running) State = OnTick();
        if (State == NodeState.Success) OnSuccess();
        else if (State == NodeState.Failure) OnFailure();
    }

    public void Print()
    {
        Debug.Log(name);
        foreach (CardEffectNode child in GetChildren()) child.Print();
    }


    public abstract void Remove(CardEffectNode node);
    public abstract void Add(CardEffectNode node);
    public abstract List<CardEffectNode> GetChildren();
    public abstract void SetChildren(params CardEffectNode[] children);

    public virtual NodeState OnStart()
    {
        return NodeState.Running;
    }

    public abstract NodeState OnTick();

    public virtual void OnStop()
    {
    }

    public virtual void OnSuccess()
    {
        OnStop();
    }

    public virtual void OnFailure()
    {
        OnStop();
    }

    public virtual void SortChildrenByNodeGraphPosition()
    {
    }

    public CardEffectNode ShallowClone()
    {
        return Instantiate(this).NewGUID();
    }

    public CardEffectNode DeepClone(CardEffectNode cloneParent)
    {
        CardEffectNode clone = ShallowClone();
        clone.parent = cloneParent;
        List<CardEffectNode> children = GetChildren();
        if (children == null || !children.Any()) return clone;
        List<CardEffectNode> clonedChildren = new();
        foreach (CardEffectNode child in children) clonedChildren.Add(child.DeepClone(clone));
        clone.SetChildren(clonedChildren.ToArray());
        return clone;
    }

    public CardEffectRootNode GetRoot()
    {
        if (this is CardEffectRootNode root) return root;
        if (parent == null) return null;
        return parent.GetRoot();
    }
}

internal static class NodeExtentions
{
    public static CardEffectNode NewGUID(this CardEffectNode node)
    {
        node.GUID = GUID.Generate().ToString();
        return node;
    }
}