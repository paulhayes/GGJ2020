using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] GameState _gameState;

    [SerializeField] AnimationCurve fadeCurve;

    [SerializeField] float holdDuration;
    [SerializeField] float fadeInDuration;

    void Awake()
    {
        _gameState.StateChangedEvent += OnGameStateChanged;
    }

    private void OnGameStateChanged(States oldState, States newState)
    {
        if(newState==States.Begining){
            CutToBlack();
            StartCoroutine( FadeIn() );
        }
    }

    public IEnumerator FadeIn()
    {        
        yield return new WaitForSeconds(holdDuration);
        var col = background.color;
        float alpha = col.a;
        float startAlpha = alpha;
        float duration = fadeInDuration;
        while(duration>0){
            alpha = fadeCurve.Evaluate(duration/fadeInDuration);
            col.a = alpha;
            background.color = col;
            yield return null;
            duration -= Time.deltaTime;
        }
    }

    public void CutToBlack()
    {
        var col = background.color;
        col.a = 1;
        background.color = col;
    }
}
