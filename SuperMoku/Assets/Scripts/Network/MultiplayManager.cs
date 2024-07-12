using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayManager : MonoBehaviour
{
    public static MultiplayManager _instance = null;

    [SerializeField]
    private GameObject multiMenu, multiLobby;

    [SerializeField]
    private GameObject chatPanel, textObject;
    [SerializeField]
    private TMP_InputField chatInputField;

    [SerializeField]
    private GameObject playerFieldBox, playerCardPrefab;
    [SerializeField]
    private GameObject readyButton, NotreadyButton, startButton;

    // Client들의 정보를 담은 Dictionary
    public Dictionary<ulong, GameObject> steamPlayerInfo = new Dictionary<ulong, GameObject>();

    [SerializeField]
    private int maxMessages = 20;
    private List<ChatMessage> chatMessageList = new List<ChatMessage>();

    public bool isConnected;
    public bool isHost;
    public ulong ownClientId;

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

    private void Update()
    {
        // 채팅 message가 있는 경우
        if(chatInputField.text != "") {
            if (Input.GetKeyDown(KeyCode.Return)) {
                // 아무것도 입력하지 않을 시 채팅 입력 창 비활성화
                if(chatInputField.text == " ") {
                    chatInputField.text = "";
                    chatInputField.DeactivateInputField();
                    return;
                }
                // 채팅 message를 Server로 전송
                NetworkTransmission._instance.SendChatMessage_ServerRPC(chatInputField.text, ownClientId);
                chatInputField.text = "";
            }
        }
        // 채팅 message가 없는 경우
        else {
            // Enter키를 통해 채팅 입력 창 활성화
            if (Input.GetKeyDown(KeyCode.Return)) {
                chatInputField.ActivateInputField();
                chatInputField.text = " ";
            }
        }
    }

    /* 구현 마무리 필요 */
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

    public class ChatMessage
    {
        public string text;
        public TMP_Text textObject;
    }

    public void SendMessageToChat(string text, ulong sender, bool isServer)
    {
        if(chatMessageList.Count >= maxMessages) {
            Destroy(chatMessageList[0].textObject.gameObject);
            chatMessageList.Remove(chatMessageList[0]);
        }
        ChatMessage newChatMessage = new ChatMessage();
        string senderName = "System";

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

        foreach(GameObject c in chat) {
            Destroy(c);
        }

        Debug.Log("clearing chat");
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

    public void AddPlayerToSteamPlayerInfo(ulong clientId, string steamName, ulong steamId)
    {
        if (!steamPlayerInfo.ContainsKey(clientId)) {
            SteamPlayerInfo pi = Instantiate(playerCardPrefab, playerFieldBox.transform).GetComponent<SteamPlayerInfo>();
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
}
