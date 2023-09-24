// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: PlayerData.cs
// Modified: 2023/04/25 @ 01:51
// Brief: Stats for player

#region

using UnityEngine;

#endregion

[CreateAssetMenu(fileName = "PlayerData", menuName = "PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    public int MaxHandSize;
    public int MaxMana;
    public int StartingHandSize;
    public int StartingHealth;
    public int StartingMana;
}