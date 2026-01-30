using Unity.Netcode;
using UnityEngine;

public class DoorBehavior : BreakableBehavior
{
    public override int hitPoints { get; set; } = 10;
}
