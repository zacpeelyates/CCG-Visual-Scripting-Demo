// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEffectDecoratorNode.cs
// Modified: 2023/05/15 @ 03:06
// Brief: Node with 1 parent and 1 child

#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

public abstract class CardEffectDecoratorNode : CardEffectNode
{
    [HideInInspector] public CardEffectNode Child;

    public override void Add(CardEffectNode node)
    {
        Child = node;
    }

    public override void Remove(CardEffectNode node)
    {
        Child = node == Child ? null : Child;
    }

    public override List<CardEffectNode> GetChildren()
    {
        return new List<CardEffectNode> { Child };
    }

    public override void SetChildren(params CardEffectNode[] children)
    {
        Child = children.FirstOrDefault();
    }

    public override void OnStop()
    {
        Child?.OnStop();
    }

    public override PortType GetInputPortType()
    {
        return PortType.Single;
    }

    public override PortType GetOutputPortType()
    {
        return PortType.Single;
    }
}