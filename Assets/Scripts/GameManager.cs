using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    enum State {
        idle,   //the "game loop" isn't running
        rhythm,  //the game loop is running, rhythm gameplay
        racing,  //the game loop is running, racing gameplay
        end  // "stop" the game loop
    }

    State gameState;

    //I recognize that this is bad architecture
    //I also do not give a FUCK
    public UnityEvent OnIdleStart, OnRhythmStart, OnRacingStart, OnEndingStart;

    void Awake(){

        //do singleton bullshit
        if(instance != this && instance != null){
            Destroy(this);
        } else {
            instance = this;
        }

        //set game state
        instance.gameState = State.idle;
    }

    void SetGameState(State state){

        if(state == State.idle){
            OnIdleStart.Invoke();
        }

        if(state == State.racing){
            OnRacingStart.Invoke();
        }

        if(state == State.racing){
            OnRhythmStart.Invoke();
        }

        if(state == State.end){
            OnEndingStart.Invoke();
        }

    }

}