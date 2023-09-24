// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: ComputerPlayer.cs
// Modified: 2023/05/15 @ 03:06

#region

using System.Collections;
using System.Linq;
using UnityEngine;

#endregion

public class ComputerPlayer : Player
{
    private readonly GameStateManager gsm = GameStateManager.GetInstance();

    public IEnumerator PerformTurn()
    {
        while (MyTurn)
        {
            yield return new WaitForSeconds(0.5f);
            TakeGameAction();
        }

        yield return null;
    }


    public void TakeGameAction()
    {
        if (gsm.IsMainPhase)
        {
            if (MathUtils.GetPCGInstance().NextBool() == false) return;
            if (hand.Count == 0) return;
            CardSlot slotToPlay = Board.instance.cardSlots.Select(x => x)
                .FirstOrDefault(x => x.owner == this && x.card == null);
            if (slotToPlay == null) return;
            var rng = MathUtils.GetPCGInstance();
            Card card = hand[rng.Next(hand.Count)];

            Board.instance.PlayCard(card, slotToPlay);
        }

        gsm.AdvanceTurnPhase();
    }
}