
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class CardEffectChoiceNode : CardEffectCompositeNode
{
    private readonly string header = "Choose Between:\n";
    private readonly string delimiter = "\nOr, ";
    public override NodeState OnStart() //Make Choice
    {
        action = () => Choice.Run();
        index = -1;
        if (!Children.Any()) return NodeState.Failure;
        if (Children.Count == 1) return Children[0].Run();
        return NodeState.Running;
    }


    private CardEffectNode Choice => choice;
    private CardEffectNode choice = null;
    private Func<NodeState> action;

    private void SetupChoiceSelection()
    {
        if (choices == null) return;
        Card baseCard = GetRoot().GetOwner().GetOwner().card;
        if (baseCard.Owner is ComputerPlayer) //make random choice for cpu
        {
            Children.GetRandomElements().First().Run();
            return;
        }
        //Setup choices
        string[] parts = GetDescriptionString(true).Remove(0,header.Length).Split(delimiter);
        Vector3 offset = Player.handSpacing * 1.25f;
        Vector3 initialPosition = (-offset * Children.Count / 2) + (Vector3.up * 0.6f);

        for (int i = 0; i < Children.Count; ++i)
        {
          
            Card card = Card.MakeBlankCard();
            card.name = $"Choice{i}";
            card.CopyCardDisplay(baseCard);
            card.cardDescriptionText.text = parts[i];
            card.gameObject.transform.position = initialPosition + offset * i;
            card.gameObject.SetActive(true); 
            choices.Add(card);
        }

        baseCard.StartCoroutine(SelectChoice());
    }

    [NonSerialized] List<Card> choices = new();

    IEnumerator SelectChoice()
    {
        while(index < 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleChoice();
            }
            yield return null;
        }
    }

    private void HandleChoice()
    {
        if(!choices.Any()) return;
        if (Physics.Raycast(MouseUtils.GetMousePosition(), Vector3.down, out RaycastHit hit, 1))
        {
            Card c;
            if ((c = hit.collider.GetComponent<Card>()) == null) return;
            if (!choices.Contains(c)) return;
            index = choices.IndexOf(c);
            choice = Children[index];
            RunChoiceAndClear();
        }
        return;
    }


    private NodeState RunChoiceAndClear()
    {
        for(int i = choices.Count - 1;  i >= 0; i--)
        {
            choices[i].gameObject.SetActive(false);
        }
        choices = null;
        return action.Invoke();
    }

    public override NodeState OnTick() 
    {
        SetupChoiceSelection();
        return NodeState.Idle;
    }

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        if (!recursive) return "Choose Between children";
        StringBuilder stringBuilder = new(header);
        foreach (var child in Children)
        {
            stringBuilder.Append(child.GetDescriptionString(true));
            if (child != Children.Last()) stringBuilder.Append(delimiter);
        }

        return stringBuilder.ToString();
    }
}
