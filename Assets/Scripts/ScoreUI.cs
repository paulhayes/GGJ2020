using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    Slider[] valueSlider;

    [SerializeField]
    Slider timerSlider;

    [SerializeField]
    TextMeshProUGUI altitudeReachedField;

    [SerializeField]
    RobotPlayerData playerData;

    [SerializeField]
    GameState _gameState;

    float lastHeightDisplayed = -1;

    void Awake()
    {
        _gameState.StateChangedEvent += OnGameStateChanged;
    }

    void OnGameStateChanged(States oldState, States newState)
    {
        timerSlider.gameObject.SetActive( _gameState.State == States.Scavenge );
        altitudeReachedField.enabled = _gameState.State == States.Launch || _gameState.State == States.Falling || _gameState.State == States.Begining;
        if(newState==States.Crash && (playerData.maxAltitudeReached<playerData.altitudeReached))
        {
            playerData.maxAltitudeReached = playerData.altitudeReached;
        }
        
    }

    void Start()
    {
        timerSlider.maxValue = playerData.levelDuration;        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<valueSlider.Length;i++){
            valueSlider[i].value = Mathf.Lerp(valueSlider[i].value, playerData.Score[i], 0.5f);
        }
        
        timerSlider.value = playerData.timeRemaining;

        var altToDisplay = (_gameState.State == States.Begining) ? playerData.maxAltitudeReached : playerData.altitudeReached;
        if(lastHeightDisplayed != altToDisplay){
            lastHeightDisplayed = altToDisplay;
            altitudeReachedField.text = Mathf.Round(lastHeightDisplayed*1000).ToString();
        }
    }
}
