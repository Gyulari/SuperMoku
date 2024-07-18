using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    private bool isSelected = false;

    public void SelectCharacter(GameObject character)
    {
        isSelected = !isSelected;
        character.transform.GetChild(0).gameObject.SetActive(isSelected);
        GameManager.m_GameData.characterName = character.name;
    }
}
