// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: MoveToZoneNode.cs
// Modified: 2023/05/21 @ 19:22

#region

using System.Collections.Generic;

#endregion

public class ReturnToHandNode : PerformGameActionNode
{
    public override void Execute(List<ITarget> targets)
    {
        foreach (ITarget target in targets)
        {
            if(target is Card card) IZone.MoveToZone(card,card.Owner.hand);
        }
    }

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        return $"Return {Targets} cards to their owners hands.";
    }
}