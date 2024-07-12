using TMPro;
using UnityEngine;

public class SteamPlayerInfo : MonoBehaviour
{
    [SerializeField]
    private TMP_Text playerName;
    public string steamName;
    public ulong steamId;

    public bool isReady;

    private void Start()
    {
        playerName.text = steamName;
    }
}
