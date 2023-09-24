// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: Board.cs
// Modified: 2023/05/15 @ 03:06

#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

public class Board : MonoBehaviour
{
    public static Board instance;

    private static readonly InPlay cardsInPlay = new();
    [SerializeField] private  List<CardSlot> enemyCardSlots = new();
    [SerializeField] private  List<CardSlot> friendlyCardSlots = new();

    [HideInInspector] public List<CardSlot> cardSlots;

    private readonly GameStateManager gsm = GameStateManager.GetInstance();

    private void Start()
    {
        if (instance != null) Destroy(gameObject);
        instance = this;
        foreach (CardSlot cardSlot in friendlyCardSlots) cardSlot.owner = gsm.player1;
        foreach (CardSlot cardSlot in enemyCardSlots) cardSlot.owner = gsm.player2;

        cardSlots = new List<CardSlot>(friendlyCardSlots);
        cardSlots.AddRange(enemyCardSlots);
    }


    public bool PlayCard(Card card, CardSlot cardSlot)
    {
        if (cardSlot.owner != card.Owner) return false;
        if (cardSlot.card != null) return false;
        card.transform.position = cardSlot.transform.position + (Vector3.up * 0.5f);
        cardSlot.card = card;
        IZone.MoveToZone(card, cardsInPlay);
        CardEffectTree tree = card.Data.GetEffectTree();
        tree.HandleKeywordData();
        List<CardEffectRootNode> roots = tree.GetRoots();
        if (roots?.Count > 0)
            foreach (TriggerRootNode root in roots.Cast<TriggerRootNode>())
                root.SetupCardCallbacks(card);

        CardCallbackEventArgs args = new() { source = card };
        GameStateManager.Invoke(GAME_TRIGGER.GameActionPlayCard, args);
        GameStateManager.Invoke(GAME_TRIGGER.TriggeredActionEnters, args);
        return true;
    }

    public List<ITarget> GetAllTargets()
    {
        return new List<ITarget> { gsm.player1, gsm.player2 }.Union(cardsInPlay).ToList();
    }
}

public class InPlay : List<Card>, IZone
{
    public Player Owner
    {
        get => null;
        set { }
    }
}