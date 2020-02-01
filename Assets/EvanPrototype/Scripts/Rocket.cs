using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rocket : MonoBehaviour
{
    [SerializeField] LayerMask _playerMask;

    [SerializeField] TextMeshPro[] _scoresText;

    Vector3 _currentScores;

    void Awake()
    {
        _currentScores = Vector3.zero;
    }

    void Update()
    {
        for (int i = 0; i < _scoresText.Length; i++)
        {
            _scoresText[i].text = _currentScores[i].ToString("0.0");
        }
    }

    public void AddScore (Vector3 scoreVector)
    {
        _currentScores += scoreVector;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_playerMask != (_playerMask | 1 << collision.gameObject.layer))
            return;

        //RobotCharacter._instance.BankItems(this);
    }
}
