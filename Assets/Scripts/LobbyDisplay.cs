using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;
#if UNITY_EDITOR
using ParrelSync;
#endif
using UnityEngine.Events;

public class LobbyDisplay : MonoBehaviour
{

    public GameObject LobbyPrefab;
    public GameObject LobbyView;

    public List<Lobby> LobbyList = new();

    public UnityAction<Lobby> OnSelectLobby;

    // Start is called before the first frame update
    async void Start()
    {
        var options = new InitializationOptions();
#if UNITY_EDITOR
        if (ClonesManager.IsClone())
        {
            Debug.Log("This is a clone project.");
            string customArgument = ClonesManager.GetArgument();
            options.SetProfile(customArgument);
        }
#endif
        AuthenticationService.Instance.SignedIn += () => GetLobbies();
    }

    public async void GetLobbies()
    {
        QueryLobbiesOptions options = new();
        options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(field: QueryFilter.FieldOptions.MaxPlayers,
                                value: "4",
                                op: QueryFilter.OpOptions.GE
                                )
            };

        var result = await Lobbies.Instance.QueryLobbiesAsync(options);
        foreach (var lobby in result.Results)
        {
            print($"Found {lobby.Name}");
            var item = Instantiate(LobbyPrefab, LobbyView.transform);
            item.GetComponentsInChildren<TMP_Text>()[0].text = lobby.Name;
            item.GetComponentsInChildren<TMP_Text>()[1].text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
