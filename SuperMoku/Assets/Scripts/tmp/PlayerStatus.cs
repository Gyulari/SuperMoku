using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : PlayerController
{
    public string m_PlayerName;

    public int HP
    {
        get { return m_HP; }
        set { m_HP = value; }
    }
    private int m_HP;

    private void Awake()
    {
        // m_PlayerName = GameManager.m_GameData.characterName;
    }
}
