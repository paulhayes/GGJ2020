using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { Speed, SlotIncrease, BatteryIncrease }
public class PowerUpItem : Item
{
    public PowerUpType _type;
}
