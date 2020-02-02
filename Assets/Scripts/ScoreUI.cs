using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    Slider[] valueSlider;

    [SerializeField]
    Slider timerSlider;

    [SerializeField]
    RobotPlayerData playerData;

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
    }
}
