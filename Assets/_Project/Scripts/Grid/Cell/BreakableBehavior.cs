using System;
using UnityEngine;

public class BreakableBehavior : UpgradableBehavior
{
    [SerializeField]
    protected int hitPoints = 2;

    public int HitPoints => hitPoints;

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
    }
}