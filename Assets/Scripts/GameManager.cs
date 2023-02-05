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

public class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; private set; }
    public CancellationTokenSource cancelSource = new();
    public TMP_Text Countdown_Text;
    public float Countdown;
    public int PlayerCount = 0;


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

        //set game state
        SetGameState((int)State.IDLE);
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

    private void Update()
    {
        if (!IsServer) return;
        if(NetworkManager.ConnectedClientsList.Count == 2 &&
            gameState == State.IDLE)
        {
            StartGameClientRPC();
        }
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

    private void OnDisable()
    {
        cancelSource.Cancel();
    }

}