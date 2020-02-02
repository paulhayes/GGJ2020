using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Rocket _rocket;

    [SerializeField]
    Rigidbody2D _rocketBody;

    Transform position;

    [SerializeField]
    float rocketSpeedToCameraDistance;

    Vector3 cameraToShipPos;

    void Awake()
    {
        cameraToShipPos = transform.position - _rocket.transform.position;
    }

    void FixedUpdate(){
        Vector3 destPos = ( _rocket.transform.position + cameraToShipPos ) + new Vector3(0,0,-rocketSpeedToCameraDistance*_rocketBody.velocity.magnitude);
        transform.position += 0.3f * ( destPos - transform.position );        
    }
}
