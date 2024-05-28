using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    public enum Turn
    { 
        Black,
        White
    }

    public Turn CurrentTurn
    {
        get { return curTurn; }
    }
    private Turn curTurn = Turn.Black;

    public void ChangeTurn()
    {
        if (curTurn == Turn.Black)
            curTurn = Turn.White;
        else
            curTurn = Turn.Black;
    }
}
