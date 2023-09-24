// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: TriggerRootNode.cs
// Modified: 2023/05/21 @ 19:22
// Brief: Root node that runs when triggered

#region

using UnityEngine;

#endregion

public class TriggerRootNode : CardEffectRootNode
{
    //delegate bool TriggerCondition(params object[] args);
    //[SerializeField] List<TriggerCondition> triggers = new();

    private CardCallback action;

     public bool MustMeetAllFlags = false;
     public PLAYER_CONDITION_FLAGS PlayerFlags;
     public ABILITY_TARGET_FLAGS SourceFlags;

    public GAME_TRIGGER trigger = GAME_TRIGGER.None;

    public bool TriggerOnce = false;
    public CardCallbackEventArgs LastTriggerArgs { get; private set; } = new();

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        string result = trigger.GetTriggerType() switch
        {
            GAME_TRIGGER_OFFSET.TurnPhase => $"On {PlayerFlags.Display()} {trigger.GetDisplayName()} Phase:",
            GAME_TRIGGER_OFFSET.GameAction => $"When {PlayerFlags.Display()} {trigger.GetDisplayName()}:",
            GAME_TRIGGER_OFFSET.TriggeredAction =>  $"On {PlayerFlags.Display()} turn, When {SourceFlags.TargetDescription(1,MustMeetAllFlags)} {trigger.GetDisplayName()}:",
            _ => $"Unknown Trigger: {trigger.GetDisplayName()}"
        };
        if (recursive) result += "\n" + child?.GetDescriptionString(true);
        if (TriggerOnce) result += "\n (Triggers once.)";
        return result;
    }

    public void SetupCardCallbacks(Card card) //create callback for card 
    {
        action ??= args => //assign action
        {
            bool cardIsValid = card != null && card.currentZone is InPlay;
            bool playerIsValid = PlayerFlags.MeetsTurnCondition(card.Owner);
            Card source = args.source == null ? card : args.source;
            bool sourceIsValid = card.MeetsCondition(source, SourceFlags, MustMeetAllFlags);

            if (!cardIsValid || !playerIsValid || !sourceIsValid) return;
            LastTriggerArgs = new CardCallbackEventArgs(source, args.amount, args.targets);
            Run();
            if (TriggerOnce) Unsubscribe(); //if only triggers once, cleanup after itself
        };

        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    public void Subscribe()
    {
        if (trigger == GAME_TRIGGER.None) return;
        GameStateManager.GetInstance().Subscribe(trigger, action);
    }

    public void Unsubscribe()
    {
        if (trigger == GAME_TRIGGER.None) return;
        GameStateManager.GetInstance().Unsubscribe(trigger, action);
    }
}