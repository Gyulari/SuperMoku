using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelector : MonoBehaviour
{
    public void SelectCharacter(GameObject character)
    {
        Debug.Log(character.name);
        GameManager._instance.LoadScene(2);
    }
}
