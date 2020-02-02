using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSpriteRenderer : MonoBehaviour
{
    [SerializeField] SpriteRenderer _target;

    Vector3 _relativePosition;
    void Awake()
    {
        _relativePosition = transform.position - _target.bounds.center;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position =  _target.bounds.center + _relativePosition;
    }

    /* 
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere( transform.position, 0.1f);
    }

    */
}
