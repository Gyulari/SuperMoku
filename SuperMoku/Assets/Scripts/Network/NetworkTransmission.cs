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

    // Client�κ��� ä�� message�� ����
    [ServerRpc(RequireOwnership = false)]
    public void SendChatMessage_ServerRPC(string message, ulong sender)
    {
        // ������ ä�� message�� �ٸ� Client�鿡�� ����
        ChatFromServer_ClientRPC(message, sender);
    }

    // Server�κ��� ä�� message�� ����
    [ClientRpc]
    private void ChatFromServer_ClientRPC(string message, ulong sender)
    {
        // ������ ä�� message�� ä��â�� ���
        MultiplayManager._instance.SendMessageToChat(message, sender, false);
    }

    // Client�κ��� Client �߰� ��û ����
    [ServerRpc(RequireOwnership = false)]
    public void AddClientToSteamPlayerInfo_ServerRPC(ulong steamId, string steamName, ulong clientId)
    {
        MultiplayManager._instance.SendMessageToChat($"{steamName} has joined", clientId, true);
        MultiplayManager._instance.AddPlayerToSteamPlayerInfo(clientId, steamName, steamId);
        MultiplayManager._instance.UpdateClients();
    }

    // Client�κ��� Client ���� ��û ����
    [ServerRpc(RequireOwnership = false)]
    public void RemoveClientFromSteamPlayerInfo_ServerRPC(ulong steamId)
    {
        // ������ Client ���� ��û�� �ٸ� Client�鿡�� ����
        RemoveClientFromSteamPlayerInfo_ClientRPC(steamId);
    }

    // Server�κ��� Client ���� ���� ��û ����
    [ClientRpc]
    private void RemoveClientFromSteamPlayerInfo_ClientRPC(ulong steamId)
    {
        Debug.Log("Removing Client");
        MultiplayManager._instance.RemovePlayerFromSteamPlayerInfo(steamId);
    }

    // Server�κ��� Client ���� ���� ��û ����
    [ClientRpc]
    public void UpdateClientsInfo_ClientRPC(ulong steamId, string steamName, ulong clientId)
    {
        MultiplayManager._instance.AddPlayerToSteamPlayerInfo(clientId, steamName, steamId);
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
