using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PUIManager : MonoBehaviour
{
    [SerializeField]
    private Image hostPlayerTurnImage, clientPlayerTurnImage;

    private void Awake()
    {
        TurnManager.OnTurnEnded += TurnChanged;
    }

    private void OnApplicationQuit()
    {
        TurnManager.OnTurnEnded -= TurnChanged;
    }

    private void TurnChanged()
    {
        hostPlayerTurnImage.enabled = TurnManager.hasTurn ? MultiplayManager._instance.isHost : !MultiplayManager._instance.isHost;
        clientPlayerTurnImage.enabled = TurnManager.hasTurn ? !MultiplayManager._instance.isHost : MultiplayManager._instance.isHost;
    }

    /*
    public TMP_Text m_FirstPlayerName;
    public TMP_Text m_SecondPlayerName;

    public Image m_FirstPlayerImage;
    public Image m_SecondPlayerImage;

    [SerializeField]
    PlayerStatus[] _PlayerStatus = new PlayerStatus[2];

    private void Start()
    {
        foreach (KeyValuePair<ulong, GameObject> player in MultiplayManager._instance.steamPlayerInfo) {

        }        
    }
    */
}
