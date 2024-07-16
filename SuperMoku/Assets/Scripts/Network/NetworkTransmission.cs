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
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    // Client로부터 채팅 message를 수신
    [ServerRpc(RequireOwnership = false)]
    public void SendChatMessage_ServerRPC(string message, ulong sender)
    {
        // 수신한 채팅 message를 다른 Client들에게 전송
        ChatFromServer_ClientRPC(message, sender);
    }

    // Server로부터 채팅 message를 수신
    [ClientRpc]
    private void ChatFromServer_ClientRPC(string message, ulong sender)
    {
        // 수신한 채팅 message를 채팅창에 출력
        MultiplayManager._instance.SendMessageToChat(message, sender, false);
    }

    // Client로부터 Client 추가 요청 수신
    [ServerRpc(RequireOwnership = false)]
    public void AddClientToSteamPlayerInfo_ServerRPC(ulong steamId, string steamName, ulong clientId)
    {
        MultiplayManager._instance.SendMessageToChat($"{steamName} 님이 게임에 참가했습니다.", clientId, true);
        MultiplayManager._instance.AddPlayerToSteamPlayerInfo(clientId, steamName, steamId);    // 플레이어 정보 dictionary에 Client 추가
        MultiplayManager._instance.UpdateClients();    // Client 정보 업데이트
    }

    // Client로부터 Client 제거 요청 수신
    [ServerRpc(RequireOwnership = false)]
    public void RemoveClientFromSteamPlayerInfo_ServerRPC(ulong steamId)
    {
        // 수신한 Client 제거 요청을 다른 Client들에게 전송
        RemoveClientFromSteamPlayerInfo_ClientRPC(steamId);
    }

    // Server로부터 Client 정보 제거 요청 수신
    [ClientRpc]
    private void RemoveClientFromSteamPlayerInfo_ClientRPC(ulong steamId)
    {
        Debug.Log("Removing Client");
        MultiplayManager._instance.RemovePlayerFromSteamPlayerInfo(steamId);    // 플레이어 정보 dictionary에서 Client 제거
    }

    // Server로부터 Client 정보 갱신 요청 수신
    [ClientRpc]
    public void UpdateClientsInfo_ClientRPC(ulong steamId, string steamName, ulong clientId)
    {
        MultiplayManager._instance.AddPlayerToSteamPlayerInfo(clientId, steamName, steamId);    // 플레이어 정보 dictionary에 Client 추가
    }

    // Server에 clientId에 해당하는 Client의 ready state를 전송
    [ServerRpc(RequireOwnership = false)]
    public void SetClientReadyState_ServerRPC(bool ready, ulong clientId)
    {
        UpdateClientsReadyState_ClientRPC(ready, clientId);    // Client들의 ready 상태 업데이트
    }

    // clientId에 해당하는 Client의 ready state를 업데이트
    [ClientRpc]
    private void UpdateClientsReadyState_ClientRPC(bool ready, ulong clientId)
    {
        foreach(KeyValuePair<ulong, GameObject> player in MultiplayManager._instance.steamPlayerInfo) {
            if(player.Key == clientId) {
                player.Value.GetComponent<SteamPlayerInfo>().isReady = ready;
                player.Value.GetComponent<SteamPlayerInfo>().readyImage.SetActive(ready);
                
                if (NetworkManager.Singleton.IsHost) {
                    Debug.Log(MultiplayManager._instance.CheckIfPlayersAreReady());
                }
            }
        }
    }
}
