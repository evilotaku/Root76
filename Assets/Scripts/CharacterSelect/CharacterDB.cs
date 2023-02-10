using UnityEngine;

[CreateAssetMenu(menuName ="Characters/Database")]
public class CharacterDB : ScriptableObject
{
    public CharacterSO[] characters = new CharacterSO[0];

    public CharacterSO[] GetAllCharacters() => characters;

    public CharacterSO GetCharacterByID(int id)
    {
        foreach (var character in characters)
        {
            if(character.Id == id)
            {
                return character;
            }
        }
        return null;
    }
}
