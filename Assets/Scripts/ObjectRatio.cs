using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class ObjectRatio
{
    [SerializeField] private GameObject _object;
    public GameObject Object => _object;
    [SerializeField] private float _ratio;
    public float Ratio => _ratio;
}