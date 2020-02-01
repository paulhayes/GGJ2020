using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] LayerMask CanCollect;

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
        if (CanCollect != (CanCollect | 1 << collision.gameObject.layer))
            return;

        Debug.Log("PICKUP :)");
    }
}
