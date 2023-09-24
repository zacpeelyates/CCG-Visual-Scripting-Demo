// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEffectActionNode.cs
// Modified: 2023/04/25 @ 01:51
// Brief: Node with 1 parent and 0 children

#region

using System.Collections.Generic;

#endregion

public abstract class CardEffectActionNode : CardEffectNode
{
    public override void Add(CardEffectNode node)
    {
    }

    public override void Remove(CardEffectNode node)
    {
    }

    public override List<CardEffectNode> GetChildren()
    {
        return new List<CardEffectNode>();
    }

    public override void SetChildren(params CardEffectNode[] children)
    {
    }

    public override PortType GetInputPortType()
    {
        return PortType.Single;
    }

    public override PortType GetOutputPortType()
    {
        return PortType.None;
    }
}