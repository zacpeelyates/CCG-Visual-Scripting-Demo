// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEffectSequencerNode.cs
// Modified: 2023/05/13 @ 22:29
// Brief: Runs children until one fails

#region

using System.Text;

#endregion

public class CardEffectSequencerNode : CardEffectCompositeNode
{
    public override NodeState OnTick() //run children until one fails
    {
        if (Children.Count == 0) return NodeState.Failure;
        NodeState childState = Children[index].Run();
        if (childState != NodeState.Success) return childState;
        return ++index < Children.Count ? NodeState.Running : NodeState.Success;
    }

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        if (!recursive) return "Runs left to right until a child fails";
        StringBuilder stringBuilder = new();
        for (int i = 0; i < Children.Count; ++i)
        {
            stringBuilder.Append(Children[i].GetDescriptionString(true));
            if (i != Children.Count - 1) stringBuilder.Append(" Then, ");
        }

        return stringBuilder.ToString();
    }
}