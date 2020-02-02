using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RobotPlayerData : ScriptableObject
{
    [ReadOnly]
    Vector3 _score;

    public event System.Action<Vector3> ScoreChangedEvent;

    public Vector3 Score {
        get {
            return _score;
        }
        set {
            _score = value;
            ScoreChangedEvent(_score);
        }
    }
    public float levelDuration = 30f;

    [System.NonSerialized]
    public float timeRemaining;

    public void GameStart()
    {
    }

    public void GameEnd()
    {
        ScoreChangedEvent = null;
    }

    public void Reset(){
        Score = Vector3.zero;
        timeRemaining = levelDuration;
    }



}
