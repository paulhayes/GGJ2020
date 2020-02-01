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

    void Update()
    {
        var dx = Input.GetAxis("Horizontal");
        var dy = Input.GetAxis("Vertical");
        //transform.position += 
        body.velocity = Time.deltaTime * new Vector3(dx,0,dy);
    }
}
