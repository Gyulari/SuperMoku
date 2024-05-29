using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : PlayerController
{
    private string m_PlayerName;

    public int HP
    {
        get { return m_HP; }
        set { m_HP = value; }
    }
    private int m_HP;
}
