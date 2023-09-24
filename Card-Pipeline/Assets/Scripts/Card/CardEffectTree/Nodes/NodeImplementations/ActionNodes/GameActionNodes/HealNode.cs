// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: HealNode.cs
// Modified: 2023/05/21 @ 19:22

#region

using System.Collections.Generic;

#endregion

public class HealNode : PerformGameActionNode
{
    public override void Execute(List<ITarget> targets)
    {
        Card.Heal(Amount, targets.ToArray());
    }

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        return $"Heal {DisplayAmount} health to {DisplayTargets}. ";
    }
}