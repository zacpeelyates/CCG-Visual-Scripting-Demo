// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEffectSelectorNode.cs
// Modified: 2023/05/13 @ 22:29
// Brief: Runs children left to right until a child succeeds 

#region

using System.Linq;
using System.Text;

#endregion

public class CardEffectSelectorNode : CardEffectCompositeNode
{
    public override NodeState OnTick() //run children until success
    {
        if (Children.Count == 0) return NodeState.Failure;
        if (Children[index].Run() == NodeState.Success) return NodeState.Success;
        return ++index < Children.Count ? NodeState.Running : NodeState.Failure;
    }

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        if (!recursive) return "Runs left to right until a child succeeds";
        StringBuilder stringBuilder = new();
        foreach (var child in Children)
        {
            stringBuilder.Append(child.GetDescriptionString(true));
            if (child != Children.Last()) stringBuilder.Append("\nElse, ");
        }

        return stringBuilder.ToString();
    }
}