using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    [SerializeField] Transform _target;

    Vector3 _relativePosition;
    void Awake()
    {
        _relativePosition = transform.position - _target.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + _relativePosition;
    }
}
