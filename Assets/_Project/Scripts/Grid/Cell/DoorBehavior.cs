using Unity.Netcode;
using UnityEngine;

public class DoorBehavior : BreakableBehavior
{
    void Awake()
    {
        hitPoints = 10;
    }
}