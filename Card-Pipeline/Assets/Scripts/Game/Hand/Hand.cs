// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: Hand.cs
// Modified: 2023/05/13 @ 22:29

#region

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

#endregion

public class Hand : ObservableCollection<Card>, IZone
{
    private Vector3 offset;
    private Vector3 origin;

    public Player Owner { get; set; }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetOffset(Vector3 offset)
    {
        this.offset = offset;
    }

    public void Render()
    {
        for (int i = 0; i < Count; i++)
        {
            Card card = this[i];
            card.transform.position = origin + offset * i;
        }
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);
        if (e.NewItems is { Count: > 0 })
            foreach (Card card in e.NewItems) card.Owner = Owner;
        Render(); //re-render hand when it changes 
    }
}