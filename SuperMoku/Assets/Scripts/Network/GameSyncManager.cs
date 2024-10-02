using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameSyncManager : MonoBehaviour
{
    public static GameSyncManager _instance;

    public GameObject TestObject;

    [SerializeField]
    private PlayerController _PlayerController;
    
    private List<ulong> playerId = new List<ulong>();

    public TMP_Text steamIdText;
    public TMP_Text prePlayerText;

    private void Awake()
    {
        _instance = this;

        foreach(KeyValuePair<ulong, GameObject> player in MultiplayManager._instance.steamPlayerInfo) {
            playerId.Add(player.Key);
        }
    }

    public void ChangeTurn(ulong prePlayer)
    {
        steamIdText.text = SteamClient.SteamId.ToString();
        prePlayerText.text = prePlayer.ToString();
        TestObject.SetActive(false);
    }
}
