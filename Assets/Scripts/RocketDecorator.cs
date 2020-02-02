using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketDecorator : MonoBehaviour
{
    [SerializeField] GameState _gameState;

    [SerializeField] RobotPlayerData playerData;

    [SerializeField] SpriteRenderer _bodyRenderer;
    [SerializeField] SpriteRenderer _engineRenderer;

    [SerializeField] Sprite[] _engineSprites;
    [SerializeField] Sprite[] _fuelSprites;

    [SerializeField] Sprite[] _bodySprites;

    [SerializeField] Vector3 maxValues;

    void Start()
    {
        _gameState.StateChangedEvent += OnGameStateChanged;
        playerData.ScoreChangedEvent += OnScoreChanged;
    }

    void OnGameStateChanged(States oldState, States newState)
    {
        if(newState==States.Crash){
            _bodyRenderer.sprite = _bodySprites[0];
        }
    }

    
    void OnScoreChanged(Vector3 score)
    {
        Vector3 normalizedScore = new Vector3(score.x/maxValues.x,score.y/maxValues.y,score.z/maxValues.z);
        Vector3Int frames = new Vector3Int(
            Math.Min( Mathf.FloorToInt(normalizedScore.x*_engineSprites.Length),_engineSprites.Length-1),
            Math.Min( Mathf.FloorToInt(normalizedScore.y*_fuelSprites.Length),_fuelSprites.Length-1),
            Math.Min( Mathf.CeilToInt(normalizedScore.z*_bodySprites.Length),_bodySprites.Length-1)
        );
        _bodyRenderer.sprite = _bodySprites[frames.z];
    }
}
