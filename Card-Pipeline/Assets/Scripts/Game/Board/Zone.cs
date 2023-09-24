// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: Zone.cs
// Modified: 2023/05/13 @ 22:29

#region

using System.Collections.Generic;
using UnityEngine;

#endregion

public interface IZone : ICollection<Card>
{
    //interface for places cards can be (Deck, Hand, InPlay, Graveyard, Exile, etc etc etc )
    Player Owner { get; set; }

    public void AddCard(Card card)
    {
        Add(card);
        card.GetComponent<CardController>().currentZone = this;
    }

    public void RemoveCard(Card card)
    {
        Remove(card);
        card.GetComponent<CardController>().currentZone = null;
    }

    public static void MoveToZone(Card card, IZone from, IZone to)
    {
        from.RemoveCard(card);
        to.AddCard(card);
    }

    public static void MoveToZone(Card card, IZone to)
    {
        card.currentZone?.RemoveCard(card);
        to.AddCard(card);
    }


}