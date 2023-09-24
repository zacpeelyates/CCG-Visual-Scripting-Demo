// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardExtentions.cs
// Modified: 2023/05/21 @ 19:22
// Breif: Card extentions that perform game actions

#region

using System.Collections.Generic;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

#endregion

public static class CardExtentions
{
    public static bool HandleAttack(this Card attacker, ITarget target)
    {
        if (!target.CanBeAttacked) return false;
        DealDamage(attacker, attacker.Data.Attack, target);
        GameStateManager.Invoke(GAME_TRIGGER.TriggeredActionDealCombatDamage,
            new CardCallbackEventArgs(attacker, attacker.Data.Attack, target.SingleToArray()));
        return true;
    }

    public static void DealDamage(this Card source, int amount, params ITarget[] targets)
    {
        if (amount <= 0) return;
        if (!targets.Any()) return;
        foreach (ITarget target in targets) DealDamage(source, amount, target);
    }

    public static void DealDamage(this Card source, int amount, ITarget target)
    {
        if (amount <= 0) return;
        CardCallbackEventArgs args = new(source, amount, target.SingleToArray());
        target.IncrementHealth(-amount);
        GameStateManager.Invoke(GAME_TRIGGER.TriggeredActionDealDamage, args);
        GameStateManager.Invoke(GAME_TRIGGER.TriggeredActionReceiveDamage, args);
        if (target.Health <= 0) target.Die(source);
    }

    public static void Heal(this Card source, int amount, params ITarget[] targets)
    {
        if (amount <= 0) return;
        if (!targets.Any()) return;
        foreach (ITarget target in targets) Heal(source, target, amount);
    }

    public static void Heal(this Card source, ITarget target, int amount)
    {
        if (amount <= 0) return;
        CardCallbackEventArgs args = new(source, amount, target.SingleToArray());
        target.IncrementHealth(amount);
        GameStateManager.Invoke(GAME_TRIGGER.TriggeredActionHealed, args);
    }

    public static void Draw(this Card source, int amount, ITarget[] targets)
    {
        if (amount <= 0) return;
        if (!targets.Any()) return;
        for (int i = 0; i < amount; i++) source.DrawCard();
    }

    public static void DrawCard(this Card card)
    {
        card.Owner.DrawCard();
    }

    public static void Buff(this Card source, int amount, ITarget[] targets)
    {
        if(amount <= 0) return;
        if (!targets.Any()) return;
        foreach (ITarget target in targets)
        {
            if (target is not Card card) continue;
            source.Buff(amount, card);
        }
    }

    public static void Buff(this Card source, int amount, Card target)
    {
        if(amount <= 0) return;
        if(target == null) return;
        CardCallbackEventArgs args = new(source, amount, target.SingleToArray());
        target.Data.Attack += amount;
        GameStateManager.Invoke(GAME_TRIGGER.TriggeredActionBuffed, args);
    }

    public static void Summon(this Card source, CardData data, int amount)
    {
        if (amount <= 0) return;
        List<CardSlot> availableSlots = Board.instance.cardSlots.Where
            (x => x.owner == source.Owner && x.card == null).ToList();
        if (availableSlots.Count == 0) return;
        for (int i = 0; i < amount && i < availableSlots.Count; ++i)
        {
            Card card = Card.MakeCardFromData(data);
            card.Owner = source.Owner;
            Board.instance.PlayCard(card, availableSlots[i]);
        }

        CardCallbackEventArgs args = new(source, amount, data.card.SingleToArray());
        GameStateManager.Invoke(GAME_TRIGGER.TriggeredActionEnters, args);
    }
}