using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownMain : MonoBehaviour
{
    [SerializeField]
    GameState _gameState;

    [SerializeField]
    RobotCharacter robot;

    // Start is called before the first frame update
    void Start()
    {
        _gameState.StateChangedEvent += StateChanged;
    }

    void StateChanged(States oldState, States newState)
    {
        
    }

}
