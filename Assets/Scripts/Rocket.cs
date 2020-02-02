using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField]
    GameState _state;

    [SerializeField]
    Rigidbody2D rocketBody;

    [SerializeField] RobotPlayerData robotPlayerData;

    [SerializeField] Explosion explosion;

    [SerializeField] float thrustScale;

    [SerializeField] float thrustDuration;

    [SerializeField] float groundLevel;

    [SerializeField] SpriteRenderer _spriteRenderer;

    [SerializeField] AudioSource crashSFX;
    Dictionary<States, System.Func<IEnumerator>> stateChangeActions = new Dictionary<States, System.Func<IEnumerator>>();

    void Start()
    {
        _state.StateChangedEvent += OnGameStateChanged;
        robotPlayerData.ScoreChangedEvent += OnScoreChanged;
        stateChangeActions.Add(States.Scavenge,StartScavenging);
        stateChangeActions.Add(States.Falling,StartFalling);
        stateChangeActions.Add(States.Crash,StartCrash);
        stateChangeActions.Add(States.Launch,StartLaunch);

    }

    private void OnScoreChanged(Vector3 score)
    {
        if(score.z>0){
            rocketBody.rotation = 0;
        }
    }

    void OnGameStateChanged(States oldState, States newState)
    {
        //Debug.LogFormat("{0} {1}",oldState,newState);
        if (stateChangeActions.ContainsKey(newState))
        {
            StartCoroutine(stateChangeActions[newState]());
        }
    }

    private IEnumerator StartScavenging()
    {
        rocketBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForSeconds(2);
        _isReady = true;
    }

    private bool _isReady;
    public bool IsReady()
    {
        return _isReady;
    }

    IEnumerator StartLaunch()
    {
        rocketBody.constraints = RigidbodyConstraints2D.None;
        Debug.Log("Thrust");
        yield return new WaitForSeconds(0.5f);    
        var score = robotPlayerData.Score;    
        var thrust = Mathf.Pow( score.x * score.y * score.z, 1/3f );
        var duration = thrustDuration;
        if( thrust < 1f){
            Debug.Log("No resultant thrust, explode on launchpad!!");
            _state.State = States.Crash;            
            yield break;
        }
        while( duration>0 ){
            yield return new WaitForFixedUpdate();
            rocketBody.AddRelativeForce(Vector3.up * thrust*thrustScale,ForceMode2D.Force);
            rocketBody.angularVelocity += 0.06f *(Mathf.PerlinNoise(transform.position.x/3,transform.position.y/3)-0.5f);
            //rocketBody.velocity += Vector2.left * 0.06f *(Mathf.PerlinNoise(transform.position.x/3,transform.position.y/3)-0.5f);
            duration -= Time.fixedDeltaTime;
        }
        _state.State = States.Falling;
    }

    IEnumerator StartFalling()
    {
        yield break;
        
    }

    IEnumerator StartCrash()
    {
        rocketBody.constraints = RigidbodyConstraints2D.FreezeAll;
        Debug.Log("Crash");
        crashSFX.Play();
        yield return explosion.Explode();
        yield return new WaitForSeconds(0.1f);
        _state.State = States.Begining;
    }

    void FixedUpdate(){
        if(rocketBody.velocity.magnitude > 1f){
            rocketBody.rotation = Vector2.SignedAngle(Vector2.up,rocketBody.velocity);
        }
        if(_state.State==States.Launch){
            CheckAltitude();
        }
        if(_state.State==States.Falling)
        {
            //Debug.LogFormat("falling {0}",rocketBody.velocity.y);
           
            rocketBody.AddForce(100f*3f*Vector2.down,ForceMode2D.Force);

            if(transform.position.y<groundLevel){
                _state.State = States.Crash;
            }


            CheckAltitude();
        }



       
        
    }

    private void CheckAltitude()
    {
        //bounds.max.y
        var alt = (_spriteRenderer.transform.position.y - groundLevel) ;
            if(alt>robotPlayerData.altitudeReached) {
                robotPlayerData.altitudeReached = alt;
            }
    }
}
