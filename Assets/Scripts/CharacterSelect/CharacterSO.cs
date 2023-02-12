using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Character")]
public class CharacterSO : ScriptableObject
{
    public int Id = -1;
    public string DisplayName = "Display Name";
    public Sprite Icon;
    public GameObject IntroPrefab;
    public NetworkObject GameplayPrefab;
}
