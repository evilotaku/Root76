using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;
using Unity.Services.Lobbies.Models;

public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
{
    public ulong ClientID;
    public int CharacterID;

    public CharacterSelectState(ulong _clientID, int _characterID = -1)
    {
        ClientID = _clientID;
        CharacterID = _characterID;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientID);
        serializer.SerializeValue(ref CharacterID);
    }   
    public bool Equals(CharacterSelectState other)
    {
        return this.ClientID == other.ClientID &&
                this.CharacterID == other.CharacterID;
    }
}
public class CharacterSelectDisplay : NetworkBehaviour
{

    NetworkList<CharacterSelectState> players;
    public CharacterDB characterDB;

    public Transform characterHolder;
    public ChracterSelectButton selectButtonPrefab;
    public PlayerCard[] playerCards;

    public TMP_Text characterNameText;
    public GameObject characterInfoPanel;

    private void Awake()
    {
        players = new();
    }
    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            CharacterSO[] allCharacters = characterDB.GetAllCharacters();
            foreach (var item in allCharacters)
            {
                var button = Instantiate(selectButtonPrefab, characterHolder);
                button.SetCharacter(this, item);
            }

            players.OnListChanged += OnListChanged;
        }
        if (IsServer)
        {

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                OnClientConnect(client.ClientId);
            }
        }

    }

    private void OnListChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if(players.Count > i)
            {
                playerCards[i].UpdateDisplay(players[i]);
            }
            else
            {
                playerCards[i].DisableDisplay();
            }

        }
    }

    private void OnClientConnect(ulong clientID)
    {
        players.Add(new CharacterSelectState(clientID));
    }

    public void Select(CharacterSO character)
    {
        characterNameText.text = character.DisplayName;
        characterInfoPanel.SetActive(true);
        SelectServerRPC(character.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    void SelectServerRPC(int characterID, ServerRpcParams param = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID == param.Receive.SenderClientId)
            {
                players[i] = new CharacterSelectState(
                    players[i].ClientID,
                    characterID);
            }
        }
       
    }


    private void OnClientDisconnect(ulong clientID)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID == clientID)
            {
                players.RemoveAt(i);
                break;
            }
        }
    }
    public override void OnNetworkDespawn()
    {
        if(IsClient)
        {
            players.OnListChanged -= OnListChanged;
        }
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }


}
