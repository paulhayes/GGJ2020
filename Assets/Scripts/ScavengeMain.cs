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

    float _beginningTimer;

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
            _beginningTimer = 0;
        }
        

    }

    void Update(){
        if(_gameState.State == States.Begining)
        {
            _beginningTimer += Time.deltaTime;

            if (_beginningTimer > 3 && Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Fire1"))
            {
                _gameState.State = States.Scavenge;
            }
        }
        else if (_gameState.State == States.Scavenge && robot.IsReady())
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
