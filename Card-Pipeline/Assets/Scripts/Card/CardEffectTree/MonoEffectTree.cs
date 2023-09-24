// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: MonoEffectTree.cs
// Modified: 2023/05/13 @ 22:29
// Brief: Tree with only one root, used for SubTree nodes 

#region

using System.Linq;
using UnityEngine;

#endregion

[CreateAssetMenu(fileName = "new CardEffectTree", menuName = "Tree/Mono", order = 0)]

public class MonoEffectTree : CardEffectTree
{
    private CardEffectRootNode Root => rootsInternal.FirstOrDefault();

    public CardEffectRootNode GetRoot()
    {
        return Root;
    }

    public void Run()
    {
        Root.Run();
    }

    protected override bool HandleNewRoot(CardEffectRootNode node)
    {
        return (rootsInternal == null || !rootsInternal.Any() || Root == null) && base.HandleNewRoot(node);
    }
}