// Author: github.com/zacpeelyates
// Project: Card-Pipeline
// Filename: GameStateManager.cs
// Modified: 2023/05/15 @ 03:06
// Brief: Handles game state and triggers

#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

public class GameStateManager
{
    private static GameStateManager instance;
    private GAME_TRIGGER currentTurnPhase = GAME_TRIGGER.None;

    private DebugText debugText;
    public GAME_TRIGGER lastTrigger = GAME_TRIGGER.None;
    public Player player1;
    public Player player2;

    private bool playerOneTurn = true;

    private readonly Dictionary<GAME_TRIGGER, CardCallback> TriggerActionDict = new();
    public int TurnIndex;
    public List<GAME_TRIGGER> TurnPhases = GameTriggerHelper.GetValuesOfType(GAME_TRIGGER_OFFSET.TurnPhase);

    public GameStateManager()
    {
        SetupDefaultCallbacks();
    }

    public GAME_TRIGGER LastTrigger
    {
        get => lastTrigger;
        set
        {
            lastTrigger = value;
            debugText?.UpdateText();
        }
    }

    public bool PlayerOneTurn
    {
        get => playerOneTurn;
        private set
        {
            playerOneTurn = value;
            Debug.Log($"Player {(playerOneTurn ? 1 : 2)}'s turn");
        }
    }

    public GAME_TRIGGER CurrentTurnPhase
    {
        get => currentTurnPhase;
        private set
        {
            if (value == currentTurnPhase) return;
            if (value == TurnPhases.First() && CurrentTurnPhase == TurnPhases.Last()) PlayerOneTurn = !PlayerOneTurn;
            currentTurnPhase = value;
            CardCallbackEventArgs args = new();
            Invoke(currentTurnPhase, args);
        }
    }

    public bool IsMainPhase => CurrentTurnPhase is GAME_TRIGGER.TurnPhasePreCombat or GAME_TRIGGER.TurnPhasePostCombat;

    public bool IsFriendlyCombat => playerOneTurn && CurrentTurnPhase == GAME_TRIGGER.TurnPhaseCombat;

    public static GameStateManager GetInstance()
    {
        return instance ??= new GameStateManager();
    }

    public static void Invoke(GAME_TRIGGER g, CardCallbackEventArgs args)
    {
        instance.TriggerActionDict[g].Invoke(args);
    }

    public void Subscribe(GAME_TRIGGER g, CardCallback func)
    {
        TriggerActionDict[g] += func;
    }

    public void Unsubscribe(GAME_TRIGGER g, CardCallback func)
    {
        TriggerActionDict[g] -= func;
    }

    public void SetupDefaultCallbacks()
    {
        foreach (GAME_TRIGGER k in GameTriggerHelper.GetTriggerList()) TriggerActionDict[k] = _ => LastTrigger = k;
        TriggerActionDict[GAME_TRIGGER.TurnPhaseStart] += _ =>
        {
            Player current = PlayerOneTurn ? player1 : player2;
            current.DrawCard();
            current.ResetAttackState();
            if (current is ComputerPlayer com) com.StartCoroutine(com.PerformTurn());
        };
    }

    public bool IsPlayerTurn(Player player)
    {
        return playerOneTurn == (player == player1);
    }

    public Player GetCurrentPlayer()
    {
        return playerOneTurn ? player1 : player2;
    }

    public Player GetOpponent(Player p) => p == player1 ? player2 : player1;
    

    public void BeginGame()
    {
        debugText = Object.FindObjectOfType<DebugText>();
        player1 = Object.FindObjectOfType<HumanPlayer>();
        player2 = Object.FindObjectOfType<ComputerPlayer>();
        PlayerOneTurn = true;
        CurrentTurnPhase = GAME_TRIGGER.TurnPhaseStart;
    }

    public void AdvanceTurnPhase()
    {
        CurrentTurnPhase = TurnPhases[++TurnIndex % TurnPhases.Count];
    }
}