using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] LayerMask _playerMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("WHATUP");

        if (_playerMask != (_playerMask | 1 << collision.gameObject.layer))
            return;

        Player._instance.BankItems();
    }
}
