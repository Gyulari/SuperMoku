using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class FriendObject : MonoBehaviour
{
    public SteamId steamId;

    public async void Invite()
    {
        if (LobbyManager.UserInLobby) {
            LobbyManager.currentLobby.InviteFriend(steamId);
        }
        else {
            bool result = await LobbyManager.CreateLobby();
            if (result) {
                LobbyManager.currentLobby.InviteFriend(steamId);
            }
        }
    }
}
