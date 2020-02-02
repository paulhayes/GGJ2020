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
        var originalScale = battery.transform.localScale;
        battery.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(0.15f);
        battery.transform.localScale = originalScale;
        yield return new WaitForSeconds(0.15f);
        battery.transform.localScale = Vector3.zero;
        yield return new WaitForSeconds(0.15f);
        battery.transform.localScale = originalScale;
        const int increments = 100;
        var incrementAmount = playerData.timeRemaining / increments;
        for (var i = 0; i < increments; i++)
        {
            battery.value += incrementAmount;
            yield return null;
        }
    }

    void Start()
    {
        _hudRelativePos = canvas.transform.position - target.transform.position;
        battery.maxValue = playerData.levelDuration;   
    }

    void Update()
    {
        if (target.IsReady())
        {
            battery.value = playerData.timeRemaining;
        }

        canvas.transform.position = target.transform.position + _hudRelativePos;
        if(canvas.enabled && _gameState.State != States.Scavenge)
        {
            canvas.enabled = false;
        }
        if (!canvas.enabled && _gameState.State == States.Scavenge && !target.IsReady())
        {
            canvas.enabled = true;
        }
    }
}
