using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayManager : MonoBehaviour
{
    public static MultiplayManager _instance = null;

    #region Lobby Objects
    [SerializeField]
    private GameObject multiMenu, multiLobby;

    [SerializeField]
    private GameObject chatPanel, textObject;
    [SerializeField]
    private TMP_InputField chatInputField;
    private bool onChatPanel;

    [SerializeField]
    private List<GameObject> playerCardField;
    [SerializeField]
    private GameObject playerCardPrefab;

    [SerializeField]
    private GameObject readyButton, readyCancelButton, startButton;

    [SerializeField]
    private int maxMessages = 20;
    private List<ChatMessage> chatMessageList = new List<ChatMessage>();

    public bool isConnected;
    public bool isHost;
    public ulong ownClientId;
    #endregion

    // Client���� ������ ���� Dictionary
    public Dictionary<ulong, GameObject> steamPlayerInfo = new Dictionary<ulong, GameObject>();

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

    #region Chat
    private void Update()
    {
        // ä��â�� ��Ȱ��ȭ ������ ��
        if (!onChatPanel) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                onChatPanel = true;
                chatPanel.SetActive(true);
                chatInputField.ActivateInputField();
                chatInputField.text = "";
            }
        }
        // ä��â�� Ȱ��ȭ ������ ��
        else {
            // ����Ű �Է� ��
            if (Input.GetKeyDown(KeyCode.Return)) {
                // �Է��� ������ ���ٸ�
                if (chatInputField.text == "") {
                    chatInputField.text = "";
                    chatInputField.DeactivateInputField();
                }
                // �Է��� ������ �ִٸ�
                else {
                    // ä�� message�� Server�� ����
                    NetworkTransmission._instance.SendChatMessage_ServerRPC(chatInputField.text, ownClientId);
                    chatInputField.text = "";
                }
            }
            // ESCŰ �Է� ��
            if (Input.GetKeyDown(KeyCode.Escape)) {
                // �Է� ���̾��ٸ�
                if(chatInputField.interactable) {
                    chatInputField.text = "";
                    chatInputField.DeactivateInputField();
                }
                // �Է� ���� �ƴϾ��ٸ�
                else {
                    chatInputField.text = "";
                    chatInputField.DeactivateInputField();
                    chatPanel.SetActive(false);
                    onChatPanel = false;
                }
            }
        }
    }

    public class ChatMessage
    {
        public string text;
        public TMP_Text textObject;
    }

    public void SendMessageToChat(string text, ulong sender, bool isServer)
    {
        if (chatMessageList.Count >= maxMessages) {
            Destroy(chatMessageList[0].textObject.gameObject);
            chatMessageList.Remove(chatMessageList[0]);
        }
        ChatMessage newChatMessage = new ChatMessage();
        string senderName = "[�ý���]";

        if (!isServer) {
            if (steamPlayerInfo.ContainsKey(sender)) {
                name = steamPlayerInfo[sender].GetComponent<SteamPlayerInfo>().steamName;
            }
        }

        newChatMessage.text = senderName + " : " + text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newChatMessage.textObject = newText.GetComponent<TMP_Text>();
        newChatMessage.textObject.text = newChatMessage.text;

        chatMessageList.Add(newChatMessage);
    }

    public void ClearChat()
    {
        chatMessageList.Clear();
        GameObject[] chat = GameObject.FindGameObjectsWithTag("ChatMessage");

        foreach (GameObject c in chat) {
            Destroy(c);
        }

        Debug.Log("clearing chat");
    }
    #endregion

    #region Connecting
    public void HostCreated()
    {
        multiMenu.SetActive(false);
        multiLobby.SetActive(true);
        isHost = true;
        isConnected = true;
    }

    public void ConnectedAsClient()
    {
        multiMenu.SetActive(false);
        multiLobby.SetActive(true);
        isHost = false;
        isConnected = true;
    }

    public void Disconnected()
    {
        steamPlayerInfo.Clear();
        
        GameObject[] playercards = GameObject.FindGameObjectsWithTag("PlayerCard");
        foreach(GameObject card in playercards) {
            Destroy(card);
        }

        multiMenu.SetActive(true);
        multiLobby.SetActive(false);
        isHost = false;
        isConnected = false;
    }
    #endregion

    #region PlayerInfo
    public void AddPlayerToSteamPlayerInfo(ulong clientId, string steamName, ulong steamId)
    {
        if (!steamPlayerInfo.ContainsKey(clientId)) {
            SteamPlayerInfo pi = Instantiate(playerCardPrefab, playerCardField[steamPlayerInfo.Count].transform).GetComponent<SteamPlayerInfo>();
            pi.steamId = steamId;
            pi.steamName = steamName;
            steamPlayerInfo.Add(clientId, pi.gameObject);
        }
    }

    public void UpdateClients()
    {
        foreach(KeyValuePair<ulong, GameObject> sPlayer in steamPlayerInfo) {
            ulong steamId = sPlayer.Value.GetComponent<SteamPlayerInfo>().steamId;
            string steamName = sPlayer.Value.GetComponent<SteamPlayerInfo>().steamName;
            ulong clientId = sPlayer.Key;

            NetworkTransmission._instance.UpdateClientsInfo_ClientRPC(steamId, steamName, clientId);
        }
    }

    public void RemovePlayerFromSteamPlayerInfo(ulong steamId)
    {
        GameObject value = null;
        ulong key = 100;
        foreach(KeyValuePair<ulong, GameObject> player in steamPlayerInfo) {
            if(player.Value.GetComponent<SteamPlayerInfo>().steamId == steamId) {
                value = player.Value;
                key = player.Key;
            }
        }
        if(key != 100) {
            steamPlayerInfo.Remove(key);
        }
        if(value != null) {
            Destroy(value);
        }
    }
    #endregion

    #region Ready
    public void SetReadyState(bool ready)
    {
        NetworkTransmission._instance.SetClientReadyState_ServerRPC(ready, ownClientId);
    }

    public bool CheckIfPlayersAreReady()
    {
        bool ready = false;

        if (steamPlayerInfo.Count != 2)
            return false;

        foreach(KeyValuePair<ulong, GameObject> player in steamPlayerInfo) {
            if (!player.Value.GetComponent<SteamPlayerInfo>().isReady) {
                startButton.SetActive(false);
                return false;
            }
            else {
                startButton.SetActive(true);
                ready = true;
            }
        }

        return ready;
    }
    #endregion

    public void StartGameByHost()
    {
        NetworkTransmission._instance.LoadScene_ServerRPC(3);
        // GameManager._instance.InitGameSettings();
    }
}
