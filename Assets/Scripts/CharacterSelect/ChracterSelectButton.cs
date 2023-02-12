using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChracterSelectButton : MonoBehaviour
{
    public Image iconImage;
    public bool IsDisabled { get; private set; }
    public Button button;

    CharacterSelectDisplay characterSelect;
    public CharacterSO character;

    public void SetCharacter(CharacterSelectDisplay _characterSelect, CharacterSO _character)
    {
        iconImage.sprite = _character.Icon;
        characterSelect = _characterSelect;
        character = _character;
    }

    public void SelectCharacter()
    {
        characterSelect.Select(character);
    }

    public void DisableButton()
    {
        IsDisabled = true;
        iconImage.color /= 2;
        button.interactable = false;
    }
}
