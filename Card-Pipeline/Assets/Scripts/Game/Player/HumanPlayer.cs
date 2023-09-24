// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: HumanPlayer.cs
// Modified: 2023/05/13 @ 22:29

#region

using UnityEngine;

#endregion

public class HumanPlayer : Player
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && hand.Count < basePlayerData.MaxHandSize) DrawCard();
        else if (Input.GetKeyDown(KeyCode.Return)) GameStateManager.GetInstance().AdvanceTurnPhase();
    }
}