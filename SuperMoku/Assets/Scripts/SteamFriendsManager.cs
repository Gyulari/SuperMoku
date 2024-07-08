using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks.ServerList;

public class SteamFriendsManager : MonoBehaviour
{
    public RawImage pp;
    public TMP_Text playername;

    public Transform friendsContent;
    public GameObject friendObj;

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

    public async void InitFriends()
    {
        foreach(var friend in SteamFriends.GetFriends()) {
            Debug.Log(
                $"{friend.Name}({friend.Id}) | online : {friend.IsOnline} | playing this game : {friend.IsPlayingThisGame}"
                );
            GameObject f = Instantiate(friendObj, friendsContent);
            f.GetComponentInChildren<TMP_Text>().text = friend.Name;
            AssignFriendImage(f, friend.Id);
        }
    }

    public async void AssignFriendImage(GameObject f, SteamId id)
    {
        var img = await SteamFriends.GetLargeAvatarAsync(id);
        f.GetComponentInChildren<RawImage>().texture = GetTextureFromImage(img.Value);
    }

    public static async System.Threading.Tasks.Task<Texture2D> GetTextureFromSteamIdAsync(SteamId Id)
    {
        var img = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
        Steamworks.Data.Image image = img.Value;
        Texture2D texture = new Texture2D((int)image.Width, (int)image.Height);

        for(int x=0; x<image.Width; x++) {
            for(int y=0; y<image.Height; y++) {
                var p = image.GetPixel(x, y);
                texture.SetPixel(x, (int)image.Height - y, new Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
            }
        }
        texture.Apply();
        return texture;
    }
}
