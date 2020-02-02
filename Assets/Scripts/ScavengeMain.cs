using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengeMain : MonoBehaviour
{
    [SerializeField]
    RobotPlayerData playerData;

    [SerializeField]
    GameState _gameState;

    [SerializeField]
    RobotCharacter robot;
    

    void Awake()
    {
        Physics2D.gravity = Vector2.zero;
        _gameState.StateChangedEvent += OnGameStateChanged;
        playerData.GameStart();
    }

    void OnGameStateChanged(States oldState, States newState)
    {
        if(newState==States.Begining){
            playerData.Reset();
        }
        

    }

    void Update(){
        if(_gameState.State == States.Begining)
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                _gameState.State = States.Scavenge;
            }
        }
        else if (_gameState.State == States.Scavenge)
        {
            playerData.timeRemaining -= Time.deltaTime;
            if (playerData.timeRemaining < 0)
            {
                playerData.timeRemaining = 0;
                Finish();
            }
        }
    }

    void OnDestroy()
    {
        playerData.GameEnd();
    }

    void Finish()
    {
        _gameState.State = States.Countdown;
    }
}
