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

    [SerializeField]
    float thrustDuration;

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
        //Debug.LogFormat("{0} {1}",oldState,newState);
        if (stateChangeActions.ContainsKey(newState))
        {
            StartCoroutine(stateChangeActions[newState]());
        }
    }

    IEnumerator StartLaunch()
    {
        Debug.Log("Thrust");
        yield return new WaitForSeconds(0.5f);    
        var score = robotPlayerData.score;    
        var thrust = Mathf.Pow( score.x * score.y * score.z, 1/3f );
        var duration = thrustDuration;
        if( thrust == 0){
            Debug.Log("No resultant thrust, explode on launchpad!!");
            thrust = 20;
        }
        while( duration>0 ){
            yield return new WaitForFixedUpdate();
            rocketBody.AddForce(new Vector3(0,thrust*thrustScale,0),ForceMode2D.Force);
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
