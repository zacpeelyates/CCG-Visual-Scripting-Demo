// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: Player.cs
// Modified: 2023/05/13 @ 22:29
// Brief: Base player class

#region

using TMPro;
using UnityEngine;

#endregion

public abstract class Player : MonoBehaviour, ITarget
{
    [SerializeField] private DeckData baseDeckData;
    [SerializeField] protected PlayerData basePlayerData;
    public bool canBeAttacked = true;

    public bool canBeTargeted = true;

    public Deck deck;
    public Hand hand;
    [SerializeField] private Vector3 handOffset;
    public static Vector3 handSpacing = default;

    [SerializeField] private Vector3 handPos;

    [SerializeField] private TMP_Text healthText;
    private int mana;

    public bool MyTurn
    {
        get
        {
            GameStateManager gsm = GameStateManager.GetInstance();
            return (gsm.PlayerOneTurn && gsm.player1 == this) || (!gsm.PlayerOneTurn && gsm.player2 == this);
        }
    }

    public bool CanBeTargeted
    {
        get => canBeTargeted;
        set => canBeTargeted = value;
    }

    public bool CanBeAttacked
    {
        get => canBeAttacked;
        set => canBeAttacked = value;
    }

    Player ITarget.Owner => this;

    public void UpdateDisplay()
    {
        healthText.text = Health.ToString();
    }

    public int Health { get; private set; }

    void ITarget.SetHealthInternal(int h)
    {
        Health = h;
    }

    void ITarget.Die(Card source)
    {
        Destroy(gameObject);
    }

    public void ResetAttackState()
    {
        if (hand == null) return;
        if (!MyTurn) return;
        foreach (Card card in hand) card.GetComponent<CardController>().canAttack = true;
    }

    private void Start()
    {
        if (handSpacing == default) handSpacing = handOffset;
        hand = new Hand();
        hand.SetOrigin(handPos);
        hand.SetOffset(handOffset);
        hand.Owner = this;

        LoadDeck();
        deck.Shuffle();
        DrawCards(basePlayerData.StartingHandSize);

        mana = basePlayerData.StartingMana;
        Health = basePlayerData.StartingHealth;
        UpdateDisplay();
    }

    public Deck LoadDeck()
    {
        return deck = baseDeckData.ToDeck();
    }

    public void DrawCard()
    {
        deck?.DrawToZone(hand);
    }

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++) DrawCard();
    }
}