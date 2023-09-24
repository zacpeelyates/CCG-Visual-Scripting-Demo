// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: DeckData.cs
// Modified: 2023/04/25 @ 01:51

#region

using System;
using UnityEngine;

#endregion

//Evil trick to make unity serialize deck dictionary
//(done only to remove <> from class as Unity refuses to serialize those or show them in inspector)
[Serializable]
public class SerializeableDeckDictionary : GenericDictionary<CardData, int> 
{
}

[CreateAssetMenu(fileName = "New Deck", menuName = "Deck")]
public class DeckData : ScriptableObject
{
    private static readonly int MaxCopies = 4;
    public SerializeableDeckDictionary serializedDeckDictionary = new();

    public void AddCard(CardData cardData)
    {
        if (serializedDeckDictionary.ContainsKey(cardData))
        {
            if (serializedDeckDictionary[cardData] < MaxCopies) serializedDeckDictionary[cardData]++;
            else Debug.Log("Max copies of card reached");
        }
        else
        {
            serializedDeckDictionary.Add(cardData, 1);
        }
    }

    public void RemoveCard(CardData cardData)
    {
        if (serializedDeckDictionary.ContainsKey(cardData))
        {
            if (serializedDeckDictionary[cardData] > 1) serializedDeckDictionary[cardData]--;
            else serializedDeckDictionary.Remove(cardData);
        }
        else
        {
            Debug.Log("Card not in deck");
        }
    }

    private void OnValidate()
    {
        foreach (var cardKVP in serializedDeckDictionary)
            if (cardKVP.Value > MaxCopies)
                serializedDeckDictionary[cardKVP.Key] = MaxCopies;
    }
}

public static class DeckDataExtentions
{
    public static Deck ToDeck(this DeckData data) //create deck object from data 
    {
        Deck deck = new();
        foreach (var cardKVP in data.serializedDeckDictionary)
            for (int i = 0; i < cardKVP.Value; i++) deck.Push(cardKVP.Key);
        return deck;
    }
}