using System;
using Unity.Netcode;
using UnityEngine;

public abstract class UpgradableBehavior : NetworkBehaviour
{
    public NetworkVariable<int> roomId =
        new NetworkVariable<int>(
            -1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<ulong> ownerClientId =
        new NetworkVariable<ulong>(
            ulong.MaxValue,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public NetworkVariable<int> level =
        new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    [SerializeField] protected int baseUpgradeCost = int.MaxValue;

    protected bool CanUpgrade(PlayerState player)
    {
        if (!IsServer) return false;
        if (player == null) return false;
        if (player.money.Value < GetUpgradeCost()) {
            Debug.Log($"Player does not have enough money to upgrade. Required: {GetUpgradeCost()}");
            return false;
        }
        
        return true;
    }

    protected abstract int GetUpgradeCost();
    protected abstract void ApplyUpgrade();

    public virtual bool TryUpgrade(ulong clientId)
    {
        if (!IsServer) return false;
        if (ownerClientId.Value != clientId) return false;

        var player = NetworkManager.Singleton
            .ConnectedClients[clientId]
            .PlayerObject
            .GetComponent<PlayerState>();

        if (!CanUpgrade(player))
            return false;

        player.money.Value -= GetUpgradeCost();
        level.Value++;
        Debug.Log($"Upgraded {name} to level {level.Value}.");

        ApplyUpgrade();
        return true;
    }

}
