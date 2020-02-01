using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    GameState _state;

    [SerializeField]
    Dictionary<States,System.Action> stateChangeActions = new Dictionary<States, System.Action>();

    void Start()
    {
        _state.StateChangedEvent += OnChanged;
        stateChangeActions.Add(States.Falling,StartFalling);
    }

    void OnChanged(States oldState, States newState)
    {
        if( stateChangeActions.ContainsKey(newState) )
        {
            stateChangeActions[newState]();
        }
    }

    void StartFalling()
    {

    }

    void Update(){
        if(_state.State==States.Falling){
            
        }
    }


}
