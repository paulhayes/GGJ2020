using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHUD : MonoBehaviour
{
    [SerializeField] Transform target;

    [SerializeField] Canvas canvas;

    Vector3 _hudRelativePos;

    void Start()
    {
        _hudRelativePos = canvas.transform.position - target.transform.position;
    }
    void Update()
    {
        canvas.transform.position = target.transform.position + _hudRelativePos;
        
    }
}
