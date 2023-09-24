// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: DamageNode.cs
// Modified: 2023/05/21 @ 19:22

#region

using System.Collections.Generic;

#endregion

public class DamageNode : PerformGameActionNode
{
    public override void Execute(List<ITarget> targets)
    {
        Card.DealDamage(Amount, targets.ToArray());
    }

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        return $"Deal {DisplayAmount} damage to {DisplayTargets}.";
    }
}