// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: CardCallback.cs
// Modified: 2023/05/21 @ 19:22
// Breif: Data types for handling card callbacks

#region

using System;
using UnityEngine;

#endregion

public delegate void CardCallback(CardCallbackEventArgs args = null);

[Serializable]
public class CardCallbackEventArgs
{
    public int amount;
    public Card source;
    public ITarget[] targets;

    public CardCallbackEventArgs(Card source = null, int amount = 0, ITarget[] targets = null)
    {
        this.source = source;
        this.amount = amount;
        this.targets = targets;
    }
}

[Serializable]
public struct ToggleableCardCallback
{
    public ToggleableCallbackValue<Card> source;

    public ToggleableCallbackValue<int> amount;
    public ToggleableCallbackValue<ITarget[]> targets;

    public void Init()
    {
        source ??= new ToggleableCallbackValue<Card> { DisplayName = "TriggerSource" };
        amount ??= new ToggleableCallbackValue<int> { DisplayName = "TriggerAmount" };
        targets ??= new ToggleableCallbackValue<ITarget[]> { DisplayName = "TriggerTargets" };
    }

    public void SetCallbacks(Func<CardCallbackEventArgs> GetArgFunction)
    {
        if (GetArgFunction != null)
        {
            CardCallbackEventArgs args = GetArgFunction();
            if (args == null) return;
        }

        source.SetCallback(() => GetArgFunction().source);
        amount.SetCallback(() => GetArgFunction().amount);
        targets.SetCallback(() => GetArgFunction().targets);
    }
}

[Serializable]
public class ToggleableCallbackValue<ValueType>
{
    [SerializeField] private Func<ValueType> CallbackFunc;
    public string DisplayName;
    public bool useCallback;
    [SerializeField] private ValueType value;

    public ValueType Value
    {
        get => useCallback && CallbackFunc != null ? CallbackFunc() : value;
        set => this.value = value;
    }

    public ValueType ValueBackingField => value; //used in Editor only

    public void SetCallback(Func<ValueType> func)
    {
        CallbackFunc = func;
    }

    public override string ToString() //value, result of callback, or callback name as needed
    {
        if (!useCallback && value != null) return value.ToString();
        ValueType vt;
        if (!Application.isPlaying || CallbackFunc == null || (vt = CallbackFunc()) == null || vt = default)
            return $"[{DisplayName}]";
        return vt.ToString();
    }

    public ValueType InvokeCallback() //force invoke
    {
        return CallbackFunc.Invoke();
    }
}