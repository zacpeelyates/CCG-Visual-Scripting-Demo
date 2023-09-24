// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: DrawNode.cs
// Modified: 2023/05/21 @ 19:22

#region

using System.Collections.Generic;
using System.Linq;

#endregion

public class DrawCardNode : PerformGameActionNode
{
    private DrawCardNode() => mustMeetAllFlags = true;
    public override bool MustHaveTargets => true;
    
    public override void Execute(List<ITarget> targets)
    {
        HashSet<Player> players = new();
        foreach (var target in targets)
        {
            players.Add(target.Owner);
        }
        players.ToList().ForEach(x => x.DrawCards(Amount));
    }

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        return $"{DisplayTargets} Draws {DisplayAmount} cards.";
    }


}