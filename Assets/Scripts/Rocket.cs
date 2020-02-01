using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    GameState _state;

    [SerializeField]
    Rigidbody2D rocketBody;

    [SerializeField]
    RobotPlayerData robotPlayerData;

    [SerializeField]
    float thrustScale;

    Dictionary<States, System.Func<IEnumerator>> stateChangeActions = new Dictionary<States, System.Func<IEnumerator>>();

    void Start()
    {
        _state.StateChangedEvent += OnChanged;
        stateChangeActions.Add(States.Falling,StartFalling);
        stateChangeActions.Add(States.Crash,StartCrash);
        stateChangeActions.Add(States.Launch,StartLaunch);

    }

    void OnChanged(States oldState, States newState)
    {
        if (stateChangeActions.ContainsKey(newState))
        {
            stateChangeActions[newState]();
        }
    }

    IEnumerator StartLaunch()
    {
        yield return new WaitForSeconds(3);    
        var score = robotPlayerData.score;    
        var thrust = Mathf.Pow( score.x * score.y * score.z, 1/3f );
        var duration = 5f;
        while( duration<=0 ){
            rocketBody.AddForce(new Vector3(0,thrust*thrustScale,0));
            yield return new WaitForFixedUpdate();
            duration -= Time.fixedDeltaTime;
        }
    }

    IEnumerator StartFalling()
    {
        yield break;
        
    }

    IEnumerator StartCrash()
    {
        yield break;
    }

    void Update(){
        if(_state.State==States.Falling)
        {
            
        }
    }


}
