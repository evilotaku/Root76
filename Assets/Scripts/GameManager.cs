using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    enum State {
        IDLE,   //the "game loop" isn't running                 0
        WARMUP, //the game loop is about to start               1
        RHYTHM,  //the game loop is running, rhythm gameplay    2
        RACE,  //the game loop is running, racing gameplay    3
        END  // "stop" the game loop                            4
    }

    State gameState;

    //I recognize that this is bad architecture
    //I also do not give a FUCK
    public UnityEvent OnIdleStart, OnWarmupStart, OnRhythmStart, OnRacingStart, OnEndingStart;

    void Awake(){

        //do singleton bullshit
        if(instance != this && instance != null){
            Destroy(this);
        } else {
            instance = this;
        }

        //set game state
        instance.gameState = State.IDLE;
    }

    public void SetGameState(int state){
        switch(state){
            // set game loop to IDLE
            case 0:
                instance.gameState = State.IDLE;
                OnIdleStart.Invoke();
                break;
            // set game loop to WARMUP
            case 1:
                instance.gameState = State.WARMUP;
                OnWarmupStart.Invoke();
                break;
            // set game loop to WARMUP
            case 2:
                instance.gameState = State.RHYTHM;
                OnRhythmStart.Invoke(); 
                break;
            // set game loop to RHYTHM
            case 3:
                instance.gameState = State.RACE;
                OnRacingStart.Invoke();
                break;
            // set game loop to END
            case 4:
                instance.gameState = State.END;
                OnEndingStart.Invoke();
                break;
        }
    }

}