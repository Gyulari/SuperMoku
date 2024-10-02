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

    // Singleton Pattern ����
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

    // ���� ���� �� Server���� ���� ����
    private void OnApplicationQuit()
    {
        Disconnected();
    }

    // Host�� Server�� ����
    public async void StartHost()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.StartHost();
        inLobby = true;

        // Lobby ����
        currentLobby = await SteamMatchmaking.CreateLobbyAsync();
    }

    // Client�� Server�� ����
    public void StartClient(SteamId steamId)
    {
        // ���� �� ���� ���� ���� Callback event ����
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        
        // Client�� ���� ���� target SteamId
        transport.targetSteamId = steamId;

        // NetworkManager�� ���� Client�� Server�� ����
        if (NetworkManager.Singleton.StartClient()) {
            Debug.Log("Client started.");
        }
    }

    // Host�� Server ���� �� ȣ��
    private void OnServerStarted()
    {
        Debug.Log("Host started.");
        MultiplayManager._instance.HostCreated();
    }

    // Server���� ���� ����
    public void Disconnected()
    {
        // Lobby���� ����
        currentLobby?.Leave();

        // NetworkManager�� instance�� �������� �ʴ� ���
        if(NetworkManager.Singleton == null) {
            return;
        }
        // NetworkManager�� instance�� Host�� ���
        if (NetworkManager.Singleton.IsHost) {
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }
        // NetworkManager�� instance�� Client�� ���
        else {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        }

        // NetworkManager�� instance�� Shutdown
        NetworkManager.Singleton.Shutdown();
        MultiplayManager._instance.ClearChat();
        MultiplayManager._instance.Disconnected();

        Debug.Log("Disconnected.");
    }

    // Client�� Server�� ����� �� ȣ��Ǵ� Callback �Լ�
    private void OnClientConnectedCallback(ulong clientId)
    {
        // Server�� Client �߰� ��û �۽�
        NetworkTransmission._instance.AddClientToSteamPlayerInfo_ServerRPC(SteamClient.SteamId, SteamClient.Name, clientId);
        MultiplayManager._instance.ownClientId = clientId;
        NetworkTransmission._instance.SetClientReadyState_ServerRPC(false, clientId);    // Server�� �ʱ� ready ���� �۽� (Default : false)
    }

    // Client�� Server���� ������ ������ �� ȣ��Ǵ� Callback �Լ�
    private void OnClientDisconnectCallback(ulong clientId)
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;

        // Host�� ���
        if(clientId == 0) {
            Disconnected();
        }
    }

    // �κ� ���� �� ȣ��Ǵ� �Լ�
    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if(result != Result.OK) {
            Debug.LogWarning("Lobby was not created.");
            return;
        }

        lobby.SetPublic();  // ���� �κ�� ����
        lobby.SetJoinable(true);  // �κ� ���� �������� ����
        lobby.SetGameServer(lobby.Owner.Id);  // �κ��� Owner�� �κ�� ������ ���� ������ ����
        Debug.Log($"Lobby was created by {SteamClient.Name}");
        
        // Server�� Client �߰� ��û �۽�
        NetworkTransmission._instance.AddClientToSteamPlayerInfo_ServerRPC(SteamClient.SteamId, SteamClient.Name, NetworkManager.Singleton.LocalClientId);
    }

    // �κ� ���� �� ȣ��Ǵ� �Լ�
    private void OnLobbyEntered(Lobby lobby)
    {
        // Host�� ���
        if (NetworkManager.Singleton.IsHost) {
            return;
        }

        // Client�� Server�� ����
        StartClient(currentLobby.Value.Owner.Id);
    }

    // �÷��̾ �κ� ���� �� ȣ��Ǵ� �Լ�
    private void OnLobbyMemberJoined(Lobby lobby, Friend steamId)
    {
        Debug.Log($"Member {steamId.Name} Joins");
    }

    // �÷��̾ �κ񿡼� ������ �� ȣ��Ǵ� �Լ�
    private void OnLobbyMemberLeave(Lobby lobby, Friend steamId)
    {
        Debug.Log($"Member {steamId.Name} Leaves");
        MultiplayManager._instance.SendMessageToChat($"{steamId.Name} ���� ������ �������ϴ�.", steamId.Id, true);
        NetworkTransmission._instance.RemoveClientFromSteamPlayerInfo_ServerRPC(steamId.Id);    // Server�� Client ���� ���� ��û �۽�
    }

    // ģ���κ��� ���� �ʴ븦 ���� �� ȣ��Ǵ� �Լ�
    private void OnLobbyInvite(Friend steamId, Lobby lobby)
    {
        Debug.Log($"Invite from {steamId.Name}");
    }

    // �κ� ���� �� ȣ��
    private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId steamId)
    {
        Debug.Log("Game was created");
        // MultiplayManager._instance.SendMessageToChat($"������ �����߽��ϴ�.", NetworkManager.Singleton.LocalClientId, true);
    }

    // �÷��̾ �κ� ���� ��û�� ���� �� ȣ��Ǵ� �Լ�
    private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
    {
        Debug.Log("Join Request");

        RoomEnter joinedLobby = await lobby.Join();

        if(joinedLobby != RoomEnter.Success) {
            Debug.LogWarning("Failed to join to lobby");
        }
        else {
            currentLobby = lobby;
            MultiplayManager._instance.ConnectedAsClient();    // Client�� �κ� Server�� ����
            Debug.Log("Join to lobby");
        }
    }
}
