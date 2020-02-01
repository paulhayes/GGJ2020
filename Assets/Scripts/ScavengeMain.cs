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
        playerData.Reset();
    }

    void Update(){
        if (_gameState.State == States.Scavenge)
        {
            playerData.timeRemaining -= Time.deltaTime;
            if (playerData.timeRemaining < 0)
            {
                playerData.timeRemaining = 0;
                Finish();
            }
        }
    }

    void Finish()
    {
        _gameState.State = States.Countdown;
    }
}
