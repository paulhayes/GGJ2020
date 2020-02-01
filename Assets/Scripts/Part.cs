using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    public readonly static int EnginePower = 0;
    public readonly static int Fuel = 1;
    public readonly static int Aerodynamic = 2;

    [Header("Engine Power, Fuel, Aerodynamic")]
    public Vector3 values;
}
