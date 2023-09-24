// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: Card.cs
// Modified: 2023/05/21 @ 19:22

#region

using TMPro;
using UnityEngine;

#endregion

public class Card : MonoBehaviour, ITarget
{
    public TMP_Text cardNameText, cardTypeText, cardDescriptionText, cardCostText, cardAttackText, cardHealthText;

    [SerializeField] private CardData inspectorCardData;

    private Player owner;

    public IZone currentZone => GetComponent<CardController>().currentZone;
    public CardData Data { get; private set; }

    public Player Owner
    {
        get => owner;
        set => owner ??= value;
    }

    public bool CanBeTargeted { get; set; } = true;
    public bool CanBeAttacked { get; set; } = true;

    public void Die(Card source)
    {
        currentZone?.Remove(this);
        CardCallbackEventArgs args = new() { source = source, targets = this.SingleToArray() };
        GameStateManager.Invoke(GAME_TRIGGER.TriggeredActionDies, args);
        Destroy(gameObject);
    }

    public int Health => Data.Health;

    public void UpdateDisplay()
    {
        RefreshCardDataDisplay();
    }

    void ITarget.SetHealthInternal(int h)
    {
        Data.Health = h;
    }

    private void Start()
    {
        if (inspectorCardData != null) LoadCardDataFromInspector();
    }

    public void LoadCardDataFromInspector()
    {
        LoadCardData(inspectorCardData, true);
    }

    public void LoadCardData(CardData baseCardData, bool newOwner)
    {
        if (baseCardData == null) return;
        baseCardData.Init();
        Data = baseCardData.Clone();
        if (newOwner) Data.card = this;
        RefreshCardDataDisplay();
    }

    public void RefreshCardDataDisplay()
    {
        cardNameText.text = Data.Name;
        cardTypeText.text = Data.Type.ToString();
        cardDescriptionText.text = ParseEffectDescription();
        cardCostText.text = Data.Cost.ToString();
        cardAttackText.text = Data.Attack.ToString();
        cardHealthText.text = Data.Health.ToString();
    }

    public void CopyCardDisplay(Card card)
    {
        cardNameText.text = card.cardNameText.text;
        cardTypeText.text = card.cardTypeText.text;
        cardDescriptionText.text = card.cardDescriptionText.text;
        cardCostText.text = card.cardCostText.text;
        cardAttackText.text = card.cardAttackText.text;
        cardHealthText.text = card.cardHealthText.text;
    }


    public string ParseEffectDescription()
    {
        //walk through effect tree and generate description of card effect
        var tree = Data.GetEffectTree();
        return tree == null ? string.Empty : tree.Parse(true);
    }

    public static Card MakeCardFromData(CardData cardData)
    {
        var card = Instantiate(VisualManager.Instance.cardPrefab).GetComponent<Card>();
        card.LoadCardData(cardData, true);
        return card;
    }

    public static Card MakeBlankCard() => Instantiate(VisualManager.Instance.cardPrefab).GetComponent<Card>();
}