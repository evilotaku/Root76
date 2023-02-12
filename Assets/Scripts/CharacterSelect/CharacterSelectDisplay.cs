using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;

public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
{
    public ulong ClientID;
    public int CharacterID;
    public bool IsReady;

    public CharacterSelectState(ulong _clientID, int _characterID = -1, bool _isReady = false)
    {
        ClientID = _clientID;
        CharacterID = _characterID;
        IsReady = _isReady;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientID);
        serializer.SerializeValue(ref CharacterID);
        serializer.SerializeValue(ref IsReady);
    }   
    public bool Equals(CharacterSelectState other)
    {
        return this.ClientID == other.ClientID &&
                this.CharacterID == other.CharacterID &&
                this.IsReady == other.IsReady;
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

    public Transform IntroSpawnPoint;
    public Button ReadyButton;
    public GameObject TmpIntro;
    List<ChracterSelectButton> characterSelectButtons = new();

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
                characterSelectButtons.Add(button);
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

        foreach (var button in characterSelectButtons)
        {
            if (button.IsDisabled) continue;
            if(IsCharacterTaken(button.character.Id, false))
            {
                button.DisableButton();
            }
        }

        foreach (var player in players)
        {
            if (player.ClientID != NetworkManager.Singleton.LocalClientId) continue;
            if(player.IsReady || IsCharacterTaken(player.CharacterID, false))
            {
                ReadyButton.interactable = false;
                break;
            }
            ReadyButton.interactable = true;
            break;
            
        }
    }

    private void OnClientConnect(ulong clientID)
    {
        players.Add(new CharacterSelectState(clientID));
    }

    public void Select(CharacterSO character)
    {
        foreach (var player in players)
        {
            if (player.ClientID != NetworkManager.Singleton.LocalClientId) continue;
            if (player.IsReady || player.CharacterID == character.Id) return;
            if (IsCharacterTaken(character.Id, false)) return;

        }
        characterNameText.text = character.DisplayName;
        characterInfoPanel.SetActive(true);
        if (TmpIntro) Destroy(TmpIntro);
        TmpIntro = Instantiate(character.IntroPrefab, IntroSpawnPoint);
        SelectServerRPC(character.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    void SelectServerRPC(int characterID, ServerRpcParams param = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!characterDB.IsValid(characterID)) return;
            if (IsCharacterTaken(characterID, true)) return;
            if (players[i].ClientID == param.Receive.SenderClientId)
            {
                players[i] = new CharacterSelectState
                    (
                        players[i].ClientID,
                        characterID,
                        players[i].IsReady
                    );
            }
        }       
    }

    public void ReadyUp()
    {
        ReadyUpServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReadyUpServerRPC(ServerRpcParams param = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID == param.Receive.SenderClientId) continue;
            if (!characterDB.IsValid(players[i].CharacterID)) return;
            if (IsCharacterTaken(players[i].CharacterID, true)) return;
            
            players[i] = new CharacterSelectState
                (
                    players[i].ClientID,
                    players[i].CharacterID,
                    _isReady: true
                );            
        }

        foreach (var player in players)
        {
            if (!player.IsReady) return;
        }

        foreach (var player in players)
        {
            GameManager.instance.SetCharacter(player.ClientID, player.CharacterID);
        }

        GameManager.instance.StartGame();
    }

    bool IsCharacterTaken(int characterID, bool CheckAll)
    {
        foreach (var player in players)
        {
            if(!CheckAll)
            {
                if (player.ClientID != NetworkManager.Singleton.LocalClientId) continue;
            }

            if(player.IsReady && player.CharacterID == characterID)
            {
                return true;
            }
        }
        return false;
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
