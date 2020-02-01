using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] int _maxTime;
    [SerializeField] TextMeshProUGUI _timeText;

    float _time;

    // Start is called before the first frame update
    void Start()
    {
        _time = _maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (_time > 0)
        {
            _time -= Time.deltaTime;
        }
        else
        {
            _time = 0;
            Player._instance.MoveEnabled(false);
        }

        _timeText.text = $"{_time.ToString("0.0")}s";
    }
}
