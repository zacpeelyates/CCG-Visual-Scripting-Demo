// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: Deck.cs
// Modified: 2023/05/13 @ 22:29

#region

using System.Collections.Generic;
using PCGSharp;

#endregion

public class Deck : Stack<CardData>
{
    private void KnuthFisherYatesShuffle()
    {
        List<CardData> list = new(this); //Convert to list
        //Randomize List with PCG instance
        Pcg pcg = MathUtils.GetPCGInstance();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int n = pcg.Next(i + 1);
            (list[n], list[i]) = (list[i], list[n]);
        }

        Clear(); //Clear stack
        list.ForEach(Push); //Push randomized list to stack
    }

    public void Shuffle() => KnuthFisherYatesShuffle();

    public bool Remove(CardData card)
    {
        if (!Contains(card)) return false;
        List<CardData> list = new(this);
        list.Remove(card);
        Clear();
        list.ForEach(Push);
        return true;
    }

    public void DrawToZone(IZone zone)
    {
        if (Count == 0 || zone == null) return;
        Card top = Card.MakeCardFromData(Pop());
        zone.AddCard(top);
    }
}