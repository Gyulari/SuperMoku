using UnityEngine;
using Unity.Netcode;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;
using Unity.VisualScripting;
using UnityEngine.Events;
using System.ComponentModel.Design;

public class GameNetworkManager : MonoBehaviour
{
    public static GameNetworkManager _instance = null;

    private FacepunchTransport transport = null;
    public Lobby? currentLobby { get; private set; } = null;

    public bool inLobby;

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

    private void Start()
    {
        transport = GetComponent<FacepunchTransport>();

        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
        SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
    }

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreated;
        SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= OnLobbyInvite;
        SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;
    }

    // 게임 종료 시 Server와의 연결 해제
    private void OnApplicationQuit()
    {
        Disconnected();
    }

    // Host로 Server에 연결
    public async void StartHost()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.StartHost();
        inLobby = true;

        // Lobby 생성
        currentLobby = await SteamMatchmaking.CreateLobbyAsync();
    }

    // Client로 Server에 연결
    public void StartClient(SteamId steamId)
    {
        // 연결 및 연결 해제 시의 Callback event 구독
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        
        // Client로 접속 시의 target SteamId
        transport.targetSteamId = steamId;

        // NetworkManager를 통해 Client로 Server에 연결
        if (NetworkManager.Singleton.StartClient()) {
            Debug.Log("Client started.");
        }
    }

    // Host로 Server 시작 시 호출
    private void OnServerStarted()
    {
        Debug.Log("Host started.");
        MultiplayManager._instance.HostCreated();
    }

    // Server와의 연결 해제
    public void Disconnected()
    {
        // Lobby에서 퇴장
        currentLobby?.Leave();

        // NetworkManager의 instance가 존재하지 않는 경우
        if(NetworkManager.Singleton == null) {
            return;
        }
        // NetworkManager의 instance가 Host인 경우
        if (NetworkManager.Singleton.IsHost) {
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }
        // NetworkManager의 instance가 Client인 경우
        else {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        }

        // NetworkManager의 instance를 Shutdown
        NetworkManager.Singleton.Shutdown();
        MultiplayManager._instance.ClearChat();
        MultiplayManager._instance.Disconnected();

        Debug.Log("Disconnected.");
    }

    // Client가 Server에 연결될 시 호출되는 Callback 함수
    private void OnClientConnectedCallback(ulong clientId)
    {
        // Server로 Client 추가 요청 송신
        NetworkTransmission._instance.AddClientToSteamPlayerInfo_ServerRPC(SteamClient.SteamId, SteamClient.Name, clientId);
        MultiplayManager._instance.ownClientId = clientId;
        NetworkTransmission._instance.SetClientReadyState_ServerRPC(false, clientId);    // Server로 초기 ready 상태 송신 (Default : false)
    }

    // Client가 Server와의 연결을 해제할 시 호출되는 Callback 함수
    private void OnClientDisconnectCallback(ulong clientId)
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;

        // Host인 경우
        if(clientId == 0) {
            Disconnected();
        }
    }

    // 로비 생성 시 호출되는 함수
    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if(result != Result.OK) {
            Debug.LogWarning("Lobby was not created.");
            return;
        }

        lobby.SetPublic();  // 공개 로비로 설정
        lobby.SetJoinable(true);  // 로비를 입장 가능으로 설정
        lobby.SetGameServer(lobby.Owner.Id);  // 로비의 Owner가 로비와 연관된 게임 서버를 설정
        Debug.Log($"Lobby was created by {SteamClient.Name}");
        
        // Server로 Client 추가 요청 송신
        NetworkTransmission._instance.AddClientToSteamPlayerInfo_ServerRPC(SteamClient.SteamId, SteamClient.Name, NetworkManager.Singleton.LocalClientId);
    }

    // 로비 입장 시 호출되는 함수
    private void OnLobbyEntered(Lobby lobby)
    {
        // Host인 경우
        if (NetworkManager.Singleton.IsHost) {
            return;
        }

        // Client로 Server에 접속
        StartClient(currentLobby.Value.Owner.Id);
    }

    // 플레이어가 로비에 입장 시 호출되는 함수
    private void OnLobbyMemberJoined(Lobby lobby, Friend steamId)
    {
        Debug.Log($"Member {steamId.Name} Joins");
    }

    // 플레이어가 로비에서 퇴장할 시 호출되는 함수
    private void OnLobbyMemberLeave(Lobby lobby, Friend steamId)
    {
        Debug.Log($"Member {steamId.Name} Leaves");
        MultiplayManager._instance.SendMessageToChat($"{steamId.Name} 님이 게임을 떠났습니다.", steamId.Id, true);
        NetworkTransmission._instance.RemoveClientFromSteamPlayerInfo_ServerRPC(steamId.Id);    // Server로 Client 정보 제거 요청 송신
    }

    // 친구로부터 게임 초대를 받을 시 호출되는 함수
    private void OnLobbyInvite(Friend steamId, Lobby lobby)
    {
        Debug.Log($"Invite from {steamId.Name}");
    }

    // 로비 생성 시 호출
    private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId steamId)
    {
        Debug.Log("Game was created");
        // MultiplayManager._instance.SendMessageToChat($"대기실을 생성했습니다.", NetworkManager.Singleton.LocalClientId, true);
    }

    // 플레이어가 로비 참여 요청을 보낼 시 호출되는 함수
    private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        Debug.Log("Join Request");

        RoomEnter joinedLobby = await lobby.Join();

        if(joinedLobby != RoomEnter.Success) {
            Debug.LogWarning("Failed to join to lobby");
        }
        else {
            currentLobby = lobby;
            MultiplayManager._instance.ConnectedAsClient();    // Client로 로비 Server에 접속
            Debug.Log("Join to lobby");
        }
    }
}
