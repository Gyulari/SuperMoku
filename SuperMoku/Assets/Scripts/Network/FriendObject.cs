using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class FriendObject : MonoBehaviour
{
    public SteamId steamId;

    public void Invite()
    {
        if (GameNetworkManager._instance.currentLobby.HasValue && GameNetworkManager._instance.inLobby) {
            GameNetworkManager._instance.currentLobby.Value.InviteFriend(steamId);
        }
    }
}
