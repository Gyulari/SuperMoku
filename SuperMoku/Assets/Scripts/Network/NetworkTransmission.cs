using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransmission : NetworkBehaviour
{
    public static NetworkTransmission _instance;

    // Singleton Pattern ����
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

    /* ���� ������ �ʿ� */
    [ServerRpc(RequireOwnership = false)]
    public void AddClientToSteamPlayerInfo_ServerRPC(ulong steamId, string steamName, ulong clientId)
    {
        MultiplayManager._instance.SendMessageToChat($"{steamName} has joined", clientId, true); // ---------- ���� �ʿ�
        MultiplayManager._instance.AddPlayerToSteamPlayerInfoDict(clientId, steamName, steamId); // ---------- ���� �ʿ�
        MultiplayManager._instance.UpdateClients(); // ---------- ���� �ʿ�
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

    // server�� clientId�� �ش��ϴ� Client�� ready state�� ����
    [ServerRpc(RequireOwnership = false)]
    public void SetClientReadyState_ServerRPC(bool ready, ulong clientId)
    {
        UpdateClientsReadyState_ClientRPC(ready, clientId);
    }

    /* ���� ������ �ʿ� */
    // clientId�� �ش��ϴ� Client�� ready state�� ������Ʈ
    [ClientRpc]
    private void UpdateClientsReadyState_ClientRPC(bool ready, ulong clientId)
    {
        foreach(KeyValuePair<ulong, GameObject> player in MultiplayManager._instance.steamPlayerInfo) {
            if(player.Key == clientId) {
                player.Value.GetComponent<SteamPlayerInfo>().isReady = ready;
                // player.Value.GetComponent<SteamPlayerInfo>().readyImage.SetActive(ready); // ---------- ���� �ʿ�
                if (NetworkManager.Singleton.IsHost) {
                    // Debug.Log(MultiplayManager._instance.CheckIfPlayersAreReday()); // ---------- ���� �ʿ�
                }
            }
        }
    }
}
