using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField]
    RobotPlayerData playerData;

    
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
}
