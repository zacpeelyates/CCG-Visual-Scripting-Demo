// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEffectStaticNode.cs
// Modified: 2023/05/13 @ 22:29
// Brief: Node with 0 parents and 0 children -- used to store keywords 

#region

using System.Collections.Generic;

#endregion

public abstract class CardEffectStaticNode : CardEffectNode
{
    public override void Add(CardEffectNode node)
    {
    }

    public override List<CardEffectNode> GetChildren()
    {
        return new List<CardEffectNode>();
    }

    public override PortType GetInputPortType()
    {
        return PortType.None;
    }

    public override PortType GetOutputPortType()
    {
        return PortType.None;
    }

    public override void Remove(CardEffectNode node)
    {
    }

    public override void SetChildren(params CardEffectNode[] children)
    {
    }
}