// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: SummonNode.cs
// Modified: 2023/05/21 @ 19:22

#region

using System.Collections.Generic;

#endregion

public class SummonNode : PerformGameActionNode
{
    public CardData cardToSummon;

    private SummonNode()
    {
        numberOfTargets = 0;
    }

    public override bool MustHaveTargets => false;

    public override void Execute(List<ITarget> targets)
    {
        Card.Summon(cardToSummon, Amount);
    }

    protected override string GetDescriptionStringInternal(bool recursive) //displays information about summoned card
    {
        if (cardToSummon == null) return string.Empty;
        cardToSummon.Init();
        string cardToSummonEffect = string.Empty;
        CardEffectTree tree = cardToSummon.GetEffectTree();
        CardData caller = GetRoot()?.GetOwner()?.GetOwner();
        bool multi = Amount > 1;
        if (caller == cardToSummon) return $"Summon {(multi ? DisplayAmount : "a")} {(multi ? "copies" : "copy")} of this";
        if (tree != null)
        {
            cardToSummonEffect = tree.Parse(recursive);
            if (cardToSummonEffect != string.Empty) cardToSummonEffect = " with " + cardToSummonEffect;
            else cardToSummonEffect = ".";
        }

        return string.Format("Summon {0} {2}/{3} {1}{4}", multi ? DisplayAmount : "a",
            cardToSummon.Name + (multi ? "s" : string.Empty), cardToSummon.Attack, cardToSummon.Health,
            cardToSummonEffect);
    }
}