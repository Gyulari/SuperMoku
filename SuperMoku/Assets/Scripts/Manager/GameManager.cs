using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    // public static GameData m_GameData = new GameData();

    void Awake()
    {
        if(_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    /*
    public void SaveData()
    {
        IOUtil.ExportDataByJson<GameData>(m_GameData, "Data/GameData.json");
    }

    public void LoadData()
    {
        m_GameData = IOUtil.ImportDataByJson<GameData>("Data/GameData.json");
    }

    public void InitGameSettings()
    {
        NetworkTransmission._instance.SetInitialTurn_ServerRPC();
    }
    */
}
