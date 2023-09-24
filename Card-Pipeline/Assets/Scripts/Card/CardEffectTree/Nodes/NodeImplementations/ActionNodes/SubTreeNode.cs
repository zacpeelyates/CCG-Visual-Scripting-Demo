// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: SubTreeNode.cs
// Modified: 2023/05/13 @ 22:29

#region

using UnityEngine;

#endregion

public class SubTreeActionNode : CardEffectActionNode
{
    [SerializeField] private MonoEffectTree SubTree;

    public override NodeState OnTick()
    {
        return SubTree.GetRoot().Run();
    }
}