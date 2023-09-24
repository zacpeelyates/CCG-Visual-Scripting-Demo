// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: KeywordNode.cs
// Modified: 2023/05/13 @ 22:29

#region

using UnityEngine;

#endregion

public class KeywordNode : CardEffectStaticNode
{
    [SerializeField] private KeywordEffectTree KeywordTree;
    public string GetReminderText => KeywordTree.Parse(true); //print full effect rather than just name

    public KeywordEffectTree GetKeywordTree()
    {
        return KeywordTree;
    }

    public override NodeState OnTick()
    {
        return NodeState.Success;
    }

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        return KeywordTree?.name;
    }
}