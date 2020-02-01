using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RobotPlayerData : ScriptableObject
{
    public Vector3 score;
    public float levelDuration = 30f;

    [System.NonSerialized]
    public float timeRemaining;

    public void Reset(){
        score = Vector3.zero;
        timeRemaining = levelDuration;
    }



}
