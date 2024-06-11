using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    int m_SelectCount = 0;

    public void SelectCharacter(GameObject character)
    {
        character.GetComponent<Button>().interactable = false;
        GameManager.m_GameData.playerName[m_SelectCount] = character.name;

        m_SelectCount++;

        if(m_SelectCount == 2) {
            GameManager._instance.SaveData();
            GameManager._instance.LoadScene(2);
        }
    }
}
