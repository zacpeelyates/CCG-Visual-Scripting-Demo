// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEffectRootNode.cs
// Modified: 2023/05/15 @ 03:06
// Brief: Node with 0 parents and 1 child

#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

public abstract class CardEffectRootNode : CardEffectNode
{
    [SerializeField] protected CardEffectNode child;
    private CardEffectTree owner;

    public CardEffectTree SetOwner(CardEffectTree owner)
    {
        return this.owner = owner;
    }

    public CardEffectTree GetOwner()
    {
        return owner;
    }

    public override void Add(CardEffectNode node)
    {
        child = node;
    }

    public override void Remove(CardEffectNode node)
    {
        child = node == child ? null : child;
    }

    public override List<CardEffectNode> GetChildren()
    {
        return new List<CardEffectNode> { child };
    }

    public override void SetChildren(params CardEffectNode[] children)
    {
        child = children.FirstOrDefault();
    }

    public override NodeState OnTick()
    {
        return child.Run();
    }

    public override PortType GetInputPortType()
    {
        return PortType.None;
    }

    public override PortType GetOutputPortType()
    {
        return PortType.Single;
    }
}