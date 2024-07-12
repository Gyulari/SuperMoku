using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransmission : NetworkBehaviour
{
    public static NetworkTransmission _instance;

    // Singleton Pattern 구현
    void Awake()
    {
        if (_instance == null) {
            _instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendChatMessage_ServerRPC(string message, ulong sender)
    {
        ChatFromServer_ClientRPC(message, sender);
    }

    [ClientRpc]
    private void ChatFromServer_ClientRPC(string message, ulong sender)
    {
        MultiplayManager._instance.SendMessageToChat(message, sender, false);
    }

    /* 구현 마무리 필요 */
    [ServerRpc(RequireOwnership = false)]
    public void AddClientToSteamPlayerInfo_ServerRPC(ulong steamId, string steamName, ulong clientId)
    {
        MultiplayManager._instance.SendMessageToChat($"{steamName} has joined", clientId, true); // ---------- 구현 필요
        MultiplayManager._instance.AddPlayerToSteamPlayerInfoDict(clientId, steamName, steamId); // ---------- 구현 필요
        MultiplayManager._instance.UpdateClients(); // ---------- 구현 필요
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveClientFromSteamPlayerInfo_ServerRPC(ulong steamId)
    {
        RemovePlayerFromDictionaryClientRPC(steamId);
    }

    [ClientRpc]
    private void RemovePlayerFromDictionaryClientRPC(ulong steamId)
    {
        Debug.Log("Removing Client");
        MultiplayManager._instance.RemovePlayerFromDictionary(steamId);
    }

    [ClientRpc]
    public void UpdateClientsPlayerInfoClientRPC(ulong steamId, string steamName, ulong clientId)
    {
        MultiplayManager._instance.AddPlayerToSteamPlayerInfoDict(clientId, steamName, steamId);
    }

    // server에 clientId에 해당하는 Client의 ready state를 전송
    [ServerRpc(RequireOwnership = false)]
    public void SetClientReadyState_ServerRPC(bool ready, ulong clientId)
    {
        UpdateClientsReadyState_ClientRPC(ready, clientId);
    }

    /* 구현 마무리 필요 */
    // clientId에 해당하는 Client의 ready state를 업데이트
    [ClientRpc]
    private void UpdateClientsReadyState_ClientRPC(bool ready, ulong clientId)
    {
        foreach(KeyValuePair<ulong, GameObject> player in MultiplayManager._instance.steamPlayerInfo) {
            if(player.Key == clientId) {
                player.Value.GetComponent<SteamPlayerInfo>().isReady = ready;
                // player.Value.GetComponent<SteamPlayerInfo>().readyImage.SetActive(ready); // ---------- 구현 필요
                if (NetworkManager.Singleton.IsHost) {
                    // Debug.Log(MultiplayManager._instance.CheckIfPlayersAreReday()); // ---------- 구현 필요
                }
            }
        }
    }
}
