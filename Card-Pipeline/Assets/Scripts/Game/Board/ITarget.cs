// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: ITarget.cs
// Modified: 2023/05/21 @ 19:22
// Breif: Interface for all targetable entities (Cards, Players) 

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#endregion

public interface ITarget
{
    public Player Owner { get; }
    bool CanBeTargeted { get; set; }
    bool CanBeAttacked { get; set; }
    public int Health { get; }

    public ABILITY_TARGET_FLAGS GetFlags(ITarget perspective) //get flags from perspective of another entity 
    {
        ABILITY_TARGET_FLAGS flags = 0;

        if (this == perspective) flags |= ABILITY_TARGET_FLAGS.This;

        if (this is Player) flags |= ABILITY_TARGET_FLAGS.Player;
        else if (this is Card card && card.Data.Type == CardData.CardType.Creature)
            flags |= ABILITY_TARGET_FLAGS.Creature;

        if ((flags & ABILITY_TARGET_FLAGS.This) == ABILITY_TARGET_FLAGS.This || Owner == perspective?.Owner)
            flags |= ABILITY_TARGET_FLAGS.Ally;
        else flags |= ABILITY_TARGET_FLAGS.Enemy;

        return flags;
    }

    public void SetHealth(int h)
    {
        SetHealthInternal(h);
        UpdateDisplay();
    }

    public void IncrementHealth(int delta)
    {
        SetHealth(Health + delta);
    }

    public void Die(Card source = null);

    protected void SetHealthInternal(int h);

    void UpdateDisplay();
}

public static class TargetExtentions
{
    public static ITarget[] SingleToArray(this ITarget i)
    {
        return new[] { i };
    }
}

[Flags]
public enum ABILITY_TARGET_FLAGS
{
    Ally = 1 << 0,
    Enemy = 1 << 1,
    Creature = 1 << 2,
    Player = 1 << 3,
    This = 1 << 4
}

public static class AbilityTargetFlagsExtentions
{
    //Check if a target meets a condition. If MustMeetAllFlags is false, function will return true if any flag is met, otherwise returns true if all flags are set
    //E.G. if conditions are (CREATURE, ENEMY) the target must be both a Creature and Enemy to pass by default. if MustMeetAllFlags is false, the target can be a Creature or Enemy.

    private static bool MeetsCondition(this ABILITY_TARGET_FLAGS target, ABILITY_TARGET_FLAGS condition,
        bool MustMeetAllFlags = false)
    {
        return MustMeetAllFlags ? (target & condition) == condition : (target & condition) > 0;
    }

    public static bool MeetsCondition(this ITarget self, ITarget source, ABILITY_TARGET_FLAGS condition,
        bool MustMeetAllFlags = false)
    {
        return self.GetFlags(source).MeetsCondition(condition, MustMeetAllFlags);
    }

    public static bool MeetsCondition(this IEnumerable<ITarget> targets, ITarget source, ABILITY_TARGET_FLAGS condition,
        bool MustMeetAllFlags = false)
    {
        return targets.All(t => t.GetFlags(source).MeetsCondition(condition, MustMeetAllFlags));
    }

    public static bool Contains(this ABILITY_TARGET_FLAGS target, ABILITY_TARGET_FLAGS flag)
    {
        return (target & flag) == flag;
    }

    //Checks if flag will have issues when MustMeetAllFlags is true
    public static bool HasConflicts(this ABILITY_TARGET_FLAGS target)
    {
        return (target.Contains(ABILITY_TARGET_FLAGS.Ally) && target.Contains(ABILITY_TARGET_FLAGS.Enemy)) ||
               (target.Contains(ABILITY_TARGET_FLAGS.Player) && target.Contains(ABILITY_TARGET_FLAGS.Creature));
    }

    public static string TargetDescription(this ABILITY_TARGET_FLAGS targets, int numberOfTargets, bool mustMeetAllFlags) //build description of legal targets
    {
        if (targets == 0 || numberOfTargets == 0) return "No Targets";

        bool multi = numberOfTargets > 1;
        if (!mustMeetAllFlags && targets.HasAllFlags())
            return $"{(multi ? numberOfTargets : "Any")} Target{(multi ? "s" : string.Empty)}";


        if (targets.Contains(ABILITY_TARGET_FLAGS.This) && (targets.Contains(ABILITY_TARGET_FLAGS.Ally) || targets.Contains(ABILITY_TARGET_FLAGS.Player)))
            targets ^= ABILITY_TARGET_FLAGS.This; //remove redundant 'This' 

        string flagString = targets.ToString();

        StringBuilder sb = new(multi ? numberOfTargets.ToString() : targets.Contains(ABILITY_TARGET_FLAGS.This) ? string.Empty : "a");
        sb.Append(' ');
        sb.Append(mustMeetAllFlags
            ? flagString.Replace(",", string.Empty)
            : Regex.Replace(flagString, ", ([^,]+)$", " or $1"));
        if(multi) sb.Append('s');
        return sb.ToString();
    }

    public static bool HasAllFlags<EnumType>(this EnumType flags) where EnumType : Enum
    {
        return (int)(object)flags == -1;
    }

}