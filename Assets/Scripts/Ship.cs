﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] LayerMask _playerMask;

    [SerializeField]
    RobotPlayerData playerData;

    [SerializeField] AudioSource _source;
    [SerializeField] AudioClip _partBankedSfx;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision");
        Debug.Log( collision.gameObject );
        var part = collision.gameObject.GetComponent<Part>();
        if(part){
            playerData.score += part.values;
            part.gameObject.SetActive(false);
        }
    }

    public void AddScore(Vector3 scoreVector)
    {
        playerData.score += scoreVector;
        _source.PlayOneShot(_partBankedSfx);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (_playerMask != (_playerMask | 1 << collision.gameObject.layer))
            return;

        RobotCharacter._instance.BankItems(this);
    }
}
