using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengeMain : MonoBehaviour
{
    [SerializeField]
    RobotPlayerData playerData;

    enum GameState {
        Countdown,
        Finsih
    } 

    GameState _gameState;


    [SerializeField]
    RobotCharacter robot;
    

    void Awake()
    {
        Physics2D.gravity = Vector2.zero;
        playerData.Reset();
        _gameState = GameState.Countdown;
    }

    void Update(){
        if(_gameState == GameState.Countdown)
        {

            playerData.timeRemaining -= Time.deltaTime;
            if(playerData.timeRemaining<0){
                playerData.timeRemaining=0;
                Finish();
            }

        }
        else if(_gameState == GameState.Finsih)
        {            
           
        }

    }

    void Finish()
    {
        _gameState = GameState.Finsih;
        robot.ReturnToShip();
        
    }
    
}
