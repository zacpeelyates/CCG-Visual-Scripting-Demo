// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: DebugText.cs
// Modified: 2023/05/13 @ 22:29

#region

using TMPro;
using UnityEngine;

#endregion

public class DebugText : MonoBehaviour
{
    private GameStateManager gsm;
    private TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        gsm = GameStateManager.GetInstance();
    }

    public void UpdateText()
    {
        if (text != null) text.text = $"Player {(gsm.PlayerOneTurn ? 1 : 2)}'s turn\n{gsm.CurrentTurnPhase}\n{gsm.LastTrigger}";
    }
}