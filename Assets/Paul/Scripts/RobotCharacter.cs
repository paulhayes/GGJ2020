using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCharacter : MonoBehaviour
{

    [SerializeField]
    Rigidbody2D body;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        var dx = Input.GetAxis("Horizontal");
        var dy = Input.GetAxis("Vertical");
        //transform.position += 
        body.velocity = new Vector3(dx,dy,0);
    }
}
