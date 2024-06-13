using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SteamFriendsManager : MonoBehaviour
{
    public RawImage pp;
    public TMP_Text playername;

    async void Start()
    {
        if (!SteamClient.IsValid) return;

        playername.text = SteamClient.Name;
        InitFriends();
        var img = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
        pp.texture = GetTextureFromImage(img.Value);
        
    }

    public static Texture2D GetTextureFromImage(Steamworks.Data.Image image)
    {
        var texture = new Texture2D((int)image.Width, (int)image.Height);

        for(int x=0; x < image.Width; x++) {
            for(int y=0; y<image.Height; y++) {
                var p = image.GetPixel(x, y);
                texture.SetPixel(x, (int)image.Height - y, new Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
            }
        }
        texture.Apply();
        return texture;
    }

    public void InitFriends()
    {
        foreach(var friend in SteamFriends.GetFriends()) {
            Debug.Log(
                $"{friend.Name}({friend.Id}) | online : {friend.IsOnline} | playing this game : {friend.IsPlayingThisGame}"
                );

        }
    }
}
