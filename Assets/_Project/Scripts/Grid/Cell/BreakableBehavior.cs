using System;
using UnityEngine;
using Unity.Netcode;

public class BreakableBehavior : UpgradableBehavior
{
    protected NetworkVariable<int> hitPoints =
    new NetworkVariable<int>(
        2,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    public int HitPoints => hitPoints.Value;

    public void TakeDamage(int damage)
    {
        if (!IsServer) return;

        hitPoints.Value -= damage;
    }
}