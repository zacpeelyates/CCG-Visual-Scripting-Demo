// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: BuffNode.cs
// Modified: 2023/05/21 @ 19:22

#region

using System.Collections.Generic;

#endregion

public class BuffNode : PerformGameActionNode
{
    public override void Execute(List<ITarget> targets)
    {
        Card.Buff(Amount, targets.ToArray());
    }

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        return $"Grant {DisplayAmount} attack to {DisplayTargets}. ";
    }
}