using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SteamPlayerInfo : MonoBehaviour
{
    public TMP_Text playerName;
    public RawImage playerImage;
    public string steamName;
    public ulong steamId;
   
    public GameObject readyImage;
    public bool isReady;

    private async void Start()
    {
        readyImage.SetActive(false);
        playerName.text = steamName;
        var img = await SteamFriends.GetLargeAvatarAsync(steamId);
        playerImage.texture = SteamFriendsManager.GetTextureFromImage(img.Value);
    }
}
