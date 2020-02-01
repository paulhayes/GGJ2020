using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameState : ScriptableObject
{
    public event System.Action<States,States> StateChangedEvent;

    [SerializeField]
    [ReadOnly]
    States _state;

    public States State {
        get {
            return _state;
        }
        set {
            var oldState = _state;
            _state = value;
            Changed(oldState,_state);
        }
    }

    public void Reset()
    {
        State = States.Falling;
    }

    public void Stopped()
    {
        StateChangedEvent = null;
    }

    void Changed(States oldState, States newState){
        if(StateChangedEvent!=null)
            StateChangedEvent(oldState,newState);
    }


}
