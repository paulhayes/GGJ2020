using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCharacter : MonoBehaviour
{

    [SerializeField]
    Rigidbody2D _body;

    [SerializeField]
    Ship _ship;

    [SerializeField]
    Transform startPosition;

    bool _returnToShip = false;

    public void ReturnToShip(){
        _returnToShip = true;
    }

    public void Reset(){
        _returnToShip = false;
        transform.position = startPosition.position;
    }

    void Start()
    {
        Reset();
    }

    void FixedUpdate()
    {


        var dx = Input.GetAxis("Horizontal");
        var dy = Input.GetAxis("Vertical");
        //transform.position += 
        _body.velocity = new Vector3(dx,dy,0);

        if(_returnToShip){
            _body.velocity = (_ship.transform.position - transform.position ).normalized;
        }
    }
}
