// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: DebugLogNode.cs
// Modified: 2023/05/13 @ 22:29

#region

using UnityEngine;

#endregion

public class DebugLogNode : CardEffectActionNode
{
    public override NodeState OnStart()
    {
        Debug.Log("Start");
        return base.OnStart();
    }

    public override void OnSuccess()
    {
        Debug.Log("Success");
        base.OnSuccess();
    }

    public override void OnFailure()
    {
        Debug.Log("Failure");
        base.OnFailure();
    }

    public override void OnStop()
    {
        Debug.Log("Stop");
    }


    public override NodeState OnTick()
    {
        Debug.Log("Tick");
        return NodeState.Success;
    }
}