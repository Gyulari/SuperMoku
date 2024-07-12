using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

public class SteamManager : MonoBehaviour
{
    public uint appId;
    public UnityEvent SteamConnectFailed;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        try {
            // Steam client √ ±‚»≠
            Steamworks.SteamClient.Init(appId, true);
            Debug.Log("Steam is successfully connected and running!");
        }
        catch (System.Exception e) {
            Debug.Log(e.Message);
            SteamConnectFailed.Invoke();
        }
    }

    private void OnApplicationQuit()
    {
        try {
            Steamworks.SteamClient.Shutdown();
        }
        catch (System.Exception e) {
            Debug.Log(e.Message);
        }
    }
}
