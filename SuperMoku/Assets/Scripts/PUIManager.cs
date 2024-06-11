using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PUIManager : MonoBehaviour
{
    public Image m_FirstPlayerImage;
    public Image m_SecondPlayerImage;

    [SerializeField]
    PlayerStatus[] _PlayerStatus = new PlayerStatus[2];

    private void Start()
    {
        Debug.Log(_PlayerStatus[0].m_PlayerName);
        Debug.Log(_PlayerStatus[1].m_PlayerName);
    }
}
