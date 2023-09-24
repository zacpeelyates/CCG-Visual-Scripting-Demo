// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: DestroyNode.cs
// Modified: 2023/05/21 @ 19:22

#region

using System.Collections.Generic;

#endregion

public class DestroyNode : PerformGameActionNode
{
    public override void Execute(List<ITarget> targets)
    {
        targets.ForEach(t =>
        {
            if (t is Card targetCard) //stops deathtouch from killing players LOL
                targetCard.Die(Card);
        });
    }

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        return $"Destroy {DisplayAmount} {DisplayTargets}";
    }
}