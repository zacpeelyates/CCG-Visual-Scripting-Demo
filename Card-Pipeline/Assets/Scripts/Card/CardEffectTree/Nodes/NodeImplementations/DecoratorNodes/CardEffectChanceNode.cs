// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardEffectChanceNode.cs
// Modified: 2023/05/13 @ 22:29

#region

using UnityEngine;

#endregion

public class CardEffectChanceNode : CardEffectDecoratorNode
{
    [SerializeField] [Range(0, 100)] private int successChance = 50;

    public override NodeState OnTick()
    {
        return Random.Range(0f, 1f) <= successChance / 100.0f ? Child.Run() : NodeState.Failure;
    }

    protected override string GetDescriptionStringInternal(bool recursive)
    {
        return !recursive ? $"{successChance}% Chance to execute" : $"{successChance}% Chance: {Child?.GetDescriptionString(true)}";
    }
}