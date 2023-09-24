// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: SplitView.cs
// Modified: 2023/04/25 @ 01:51

#region

using UnityEngine.UIElements;

#endregion

//For some reason twopanesplitview is available in code but not in the UIBuilder.
//This adds it to the UI builder. 
public class SplitView : TwoPaneSplitView
{
    public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
    {
    }
}