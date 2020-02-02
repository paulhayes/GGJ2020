using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RobotHud : MonoBehaviour
{
    [SerializeField] private GameState _gameState;
    [SerializeField] RobotCharacter target;
    [SerializeField] RobotPlayerData playerData;

    [SerializeField] Canvas canvas;
    [SerializeField] private Slider battery;

    Vector3 _hudRelativePos;

    private void OnEnable()
    {
        _gameState.StateChangedEvent += OnStateChanged;
    }

    private void OnDisable()
    {
        _gameState.StateChangedEvent -= OnStateChanged;
    }

    private void OnStateChanged(States oldState, States newState)
    {
        if (newState == States.Scavenge)
        {
            StartCoroutine(FillBattery());
        }
    }

    private IEnumerator FillBattery()
    {
        battery.value = 0;
        const int increments = 20;
        for (var i = 0; i < increments; i++)
        {
            battery.value += 1f / increments;
            yield return null;
        }
    }

    void Start()
    {
        _hudRelativePos = canvas.transform.position - target.transform.position;
    }

    void Update()
    {
        canvas.transform.position = target.transform.position + _hudRelativePos;
        if( canvas.enabled != target.gameObject.activeSelf)
        {
            canvas.enabled = target.gameObject.activeSelf;
        }
    }
}
