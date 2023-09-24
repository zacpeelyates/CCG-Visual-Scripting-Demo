// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: PerformGameActionNode.cs
// Modified: 2023/05/21 @ 19:22
// Brief: Base class for all game action nodes

#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

public abstract class PerformGameActionNode : CardEffectActionNode
{
    public enum TargetCallbackType
    {
        Target,
        Source
    }

    [HideInInspector] public ToggleableCardCallback args;
    private Card card;

    private bool hasExecuted;

    private bool hasInit;

    [HideInInspector] public ABILITY_TARGET_FLAGS LegalTargets;

    [HideInInspector] public bool mustMeetAllFlags = false;

    [HideInInspector] public int numberOfTargets;

    [HideInInspector] public TargetCallbackType targetCallbackType;

    protected PerformGameActionNode()
    {
        Init();
    }

    public virtual bool MustHaveTargets => true;

    public int Amount
    {
        get => args.amount.Value;
        set => args.amount.Value = value;
    }

    public ITarget[] Targets => GetCallbackTargets();

    public string DisplayAmount => args.amount.ToString();
    public string DisplayTargets => targetCallbackType == TargetCallbackType.Source ? sourceString : targetString;
    private string sourceString => args.source.ToString();

    private string targetString => args.targets.useCallback
        ? args.targets.ToString()
        : LegalTargets.TargetDescription(numberOfTargets, mustMeetAllFlags);

    public Card Card => GetCard();

    private ITarget[] GetCallbackTargets()
    {
        return targetCallbackType == TargetCallbackType.Source ? args.source.Value.SingleToArray() : args.targets.Value.ToArray();
    }

    private void Init()
    {
        if (hasInit) return;
        args.Init();
        hasInit = true;
    }

    private Card GetCard()
    {
        CardEffectRootNode root = GetRoot();
        CardEffectTree tree = root.GetOwner();
        CardData cardData = tree.GetOwner();
        card = cardData.card;
        return card;
    }


    public void SetupCallback() //setup callback to root node args
    {
        CardEffectRootNode root = GetRoot();
        if (root is TriggerRootNode triggerRoot) args.SetCallbacks(() => triggerRoot.LastTriggerArgs);
    }

    protected void ExecuteInternal(List<ITarget> targets)
    {
        Execute(targets);
        hasExecuted = true;
    }

    public abstract void Execute(List<ITarget> targets);

    public override NodeState OnStart()
    {
        if (TargetHelper.Instance.CurrentlyRequestingTarget) return NodeState.Idle; //wait for target selection to complete
        hasExecuted = false;
        if (args.targets.useCallback) //get existing targets
        {
            List<ITarget> targetList = new(Targets);
            if (!targetList.Any()) Debug.LogError("No targets found in callback");
            ExecuteInternal(targetList);
        }
        else if (!MustHaveTargets || numberOfTargets == 0) //Execute with no targets 
        {
            ExecuteInternal(null);
        }
        else
        {
            int LegalTargetCount = TargetHelper.GetLegalTargetCount(Card, LegalTargets, mustMeetAllFlags, out List<ITarget> targets);
            if (numberOfTargets > LegalTargetCount) //cant assign enough targets
            {
                return NodeState.Failure;
            }

            if (numberOfTargets == LegalTargetCount) //auto-execute with all legal targets 
            {
                ExecuteInternal(targets);
            }

            else if (card.Owner is ComputerPlayer) //cpu picks random targets
            {
                ExecuteInternal(targets.GetRandomElements(numberOfTargets)); 
            }
            else if (!TargetHelper.Instance.CurrentlyRequestingTarget) //request player select targets
            {
                TargetHelper.Instance.RequestTarget(Card, LegalTargets, numberOfTargets, ExecuteInternal, mustMeetAllFlags);
                return NodeState.Idle;
            }
        }

        return base.OnStart();
    }

    public override NodeState OnTick()
    {
        return hasExecuted ? NodeState.Success : NodeState.Running;
    }

    private void OnValidate()
    {
        if (!hasInit) Init();
        numberOfTargets = Mathf.Max(MustHaveTargets ? 1 : 0, numberOfTargets);
        args.source.useCallback = args.targets.useCallback; //awful bad terrible fix but deadline is coming
    }
}