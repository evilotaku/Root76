using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using TMPro;
using UnityEditor;
using UnityEngine.UI;


[Serializable]
public struct ClientData
{
    public ulong clientID; //TODO: change this to Unity Player ID
    public int characterId;

    public ClientData(ulong _clientId, int _characterID = -1)
    {
        clientID = _clientId;
        characterId = _characterID;
    }
}

public class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; private set; }
    public CancellationTokenSource cancelSource = new();
    public TMP_Text Countdown_Text;
    public float Countdown;
    public int PlayerCount = 0;
    public Slider Timerbar;
    public NetworkObject RhythmCanvas;
    public string characterSelectScene = "scenes/CharacterSelectScene";
    public string gameplayScene = "scenes/RaceScene";
    public Dictionary<ulong, ClientData> ClientData = new();
    public bool GameHasStarted;


    enum State {
        IDLE,   //the "game loop" isn't running                 0
        WARMUP, //the game loop is about to start               1
        RACE,   //the game loop is running, racing gameplay     2
        RHYTHM, //the game loop is running, rhythm gameplay     3                
        END     // "stop" the game loop                         4
    }

    State gameState;

    //I recognize that this is bad architecture
    //I also do not give a FUCK
    public static UnityAction OnIdleStart, OnWarmupStart, OnRhythmStart, OnRacingStart, OnEndingStart;

    void Awake(){

        //do singleton bullshit
        if(instance != this && instance != null){
            Destroy(this);
        } else {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        OnWarmupStart += async () =>
        {
            Countdown_Text.gameObject.SetActive(true);
            await Timer(Countdown, new Progress<float>(percent =>
            {
                print($"Timer: {percent} ");
                Countdown_Text.text = $"{(int)(Countdown - (Countdown * percent))}";
            }), cancelSource.Token);
            Countdown_Text.text = "GO!";
            await Task.Delay(1000);
            Countdown_Text.gameObject.SetActive(false);
            NextState();
        };

        OnRhythmStart += async () =>
        {
            if (!NetworkManager.Singleton.IsServer) return;
            var rhythm = Instantiate(RhythmCanvas);
            rhythm.Spawn();
            await Timer(40f, new Progress<float>(percent =>
            {
                print($"Timer: {percent} ");
                Timerbar.value = percent;
            }), cancelSource.Token);
            rhythm.Despawn();
            SetGameState((int)State.RACE);

        };
        //set game state
        SetGameState((int)State.IDLE);
    }

    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (ClientData.Count >= NetworkConnectionManager.Instance.MaxConnections || GameHasStarted)
        {
            response.Approved = false;
            response.Reason = "Match is Full";
            return;
        }
        ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);
    }

    public void OnClientConnect(ulong clientId)
    {
        PlayerCount++;
    }

    public void SetGameState(int state)
    {
        state  %= Enum.GetNames(typeof(State)).Length;
        switch (state){
            // set game loop to IDLE
            case 0:
                instance.gameState = State.IDLE;
                OnIdleStart?.Invoke();
                break;
            // set game loop to WARMUP
            case 1:
                instance.gameState = State.WARMUP;
                OnWarmupStart?.Invoke();
                break;
            // set game loop to RHYTHM
            case 3:
                instance.gameState = State.RHYTHM;
                OnRhythmStart?.Invoke(); 
                break;
            // set game loop to RACE
            case 2:
                instance.gameState = State.RACE;
                OnRacingStart?.Invoke();
                break;
            // set game loop to END
            case 4:
                instance.gameState = State.END;
                OnEndingStart?.Invoke();
                break;
        }
    }

    public void NextState()
    {
        gameState++;
        print($"Game is now in {gameState}");
        //gameState %= Enum.GetNames(typeof(State)).Length;
        SetGameState((int)gameState);
    }

    [ClientRpc]
    public void StartGameClientRPC()
    {
        print("Starting the Countdown!");
        SetGameState((int)State.WARMUP);
    }

    public void SetCharacter(ulong clientId, int characterId)
    {
        if (ClientData.TryGetValue(clientId, out ClientData data))
        {
            data.characterId = characterId;
        }
    }   

    public void StartGame()
    {
        GameHasStarted = true;
        StartGameClientRPC();
        NetworkManager.Singleton.SceneManager.LoadScene(gameplayScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }



    private void Update()
    {
        if (!IsServer) return;
/*        if(NetworkManager.ConnectedClientsList.Count == 2 &&
            gameState == State.IDLE)
        {
            StartGameClientRPC();
        }*/
    }

    public async Task Timer(float amount, IProgress<float> progress, CancellationToken cancelToken)
    {
        for (int i = 0; i <= amount; i++)
        {
            if (cancelToken.IsCancellationRequested) break;
            await Task.Delay(1000);
            progress?.Report(i / amount);
        }
        Debug.Log("Times Up!");
    }
    public void OnClientDisconnect(ulong clientID)
    {
        ClientData.Remove(clientID);
    }

    private void OnDisable()
    {
        cancelSource.Cancel();
    }

}