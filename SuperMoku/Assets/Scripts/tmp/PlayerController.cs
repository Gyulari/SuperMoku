using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    protected TurnManager.Turn m_PlayerTurn;

    protected GameObject m_PlayerStone;

    public void Act()
    {
        // NetworkTransmission._instance.EndTurn_ServerRPC(SteamClient.SteamId);
    }
}
