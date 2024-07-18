using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SteamFriendsManager : MonoBehaviour
{
    public RawImage playerImg;
    public TMP_Text playerName;

    public Transform friendsListContent;
    public GameObject friendObj;

    [SerializeField]
    private int maxNameLength;

    async void Start()
    {
        if (!SteamClient.IsValid) return;

        playerName.text = SteamClient.Name;
        InitFriendListAsync();

        var img = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
        playerImg.texture = GetTextureFromImage(img.Value);
    }

    public static Texture2D GetTextureFromImage(Steamworks.Data.Image image)
    {
        var texture = new Texture2D((int)image.Width, (int)image.Height);

        for(int x=0; x<image.Width; x++) {
            for(int y=0; y<image.Height; y++) {
                var p = image.GetPixel(x, y);
                texture.SetPixel(x, (int)image.Height - y, new Color(p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f));
            }
        }
        texture.Apply();
        return texture;
    }

    public void InitFriendListAsync()
    {
        foreach(var friend in SteamFriends.GetFriends()) {
            GameObject fObj = Instantiate(friendObj, friendsListContent);
            fObj.GetComponentInChildren<TMP_Text>().text = friend.Name;
            fObj.GetComponent<FriendObject>().steamId = friend.Id;

            AssignFriendImage(fObj, friend.Id);
        }
    }

    public async void AssignFriendImage(GameObject fObj, SteamId id)
    {
        var img = await SteamFriends.GetLargeAvatarAsync(id);
        fObj.GetComponentInChildren<RawImage>().texture = GetTextureFromImage(img.Value);
    }

    public static async System.Threading.Tasks.Task<Texture2D> GetTextureFromSteamIdAsync(SteamId Id)
    {
        var img = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
        return GetTextureFromImage(img.Value);
    }

    public void FriendsListDrawerFunc(float x)
    {
        gameObject.GetComponent<RectTransform>().DOAnchorPosX(x, 0.3f);
    }
}
