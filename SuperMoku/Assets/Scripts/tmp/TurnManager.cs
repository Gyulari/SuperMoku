using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static event Action OnTurnEnded;

    public enum Turn
    {
        First,
        Second
    }

    public Turn CurrentTurn
    {
        get { return m_CurrentTurn; }
    }
    private Turn m_CurrentTurn = Turn.First;

    public static bool hasTurn = false;

    public void ChangeTurn()
    {

    }

    public static void EndTurn()
    {
        hasTurn = false;
        // NetworkTransmission._instance.EndTurn_ServerRPC(SteamClient.SteamId);
        OnTurnEnded.Invoke();
    }
}
