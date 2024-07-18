using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
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

    public void ChangeTurn()
    {
        if (m_CurrentTurn == Turn.First)
            m_CurrentTurn = Turn.Second;
        else
            m_CurrentTurn = Turn.First;
    }
}
