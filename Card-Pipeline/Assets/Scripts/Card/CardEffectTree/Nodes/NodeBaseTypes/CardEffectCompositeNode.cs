// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEffectCompositeNode.cs
// Modified: 2023/05/15 @ 03:06 
// Brief: Node with 1 parent and multiple children 

#region

using System.Collections.Generic;
using System.Linq;

#endregion

public abstract class CardEffectCompositeNode : CardEffectNode
{
    public List<CardEffectNode> Children = new();
    protected int index;

    public override NodeState OnStart()
    {
        index = 0;
        return base.OnStart();
    }

    public override void Add(CardEffectNode node)
    {
        Children.Add(node);
    }

    public override void Remove(CardEffectNode node)
    {
        Children.Remove(node);
    }

    public override List<CardEffectNode> GetChildren()
    {
        return Children;
    }

    public override void SetChildren(params CardEffectNode[] children)
    {
        Children = children.ToList();
    }

    public override void SortChildrenByNodeGraphPosition()
    {
        Children = Children?.OrderBy(x => x?.CachedGraphPosition.x).ToList();
    }

    public override void OnStop()
    {
        Children.ForEach(x => x.OnStop());
    }

    public override PortType GetInputPortType()
    {
        return PortType.Single;
    }

    public override PortType GetOutputPortType()
    {
        return PortType.Multi;
    }
}