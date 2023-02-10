using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCard : MonoBehaviour
{
    public CharacterDB characterDB;
    public GameObject visuals;
    public Image IconImage;
    public TMP_Text PlayerName, CharacterName;
    
    public void UpdateDisplay(CharacterSelectState state)
    {
        if(state.CharacterID != -1)
        {
            var character = characterDB.GetCharacterByID(state.CharacterID);
            IconImage.sprite = character.Icon;
            IconImage.enabled = true;
            CharacterName.text = character.DisplayName;
        }
        else
        {
            IconImage.enabled = false;
        }

        PlayerName.text = $"Player {state.ClientID}";
        visuals.SetActive(true);
    }

    public void DisableDisplay()
    {
        visuals.SetActive(false);
    }
}
