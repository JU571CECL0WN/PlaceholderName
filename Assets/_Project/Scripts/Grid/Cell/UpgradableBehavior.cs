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

    public bool HasOwner => ownerClientId.Value != ulong.MaxValue;

    public bool IsOwnedBy(ulong clientId)
    {
        return ownerClientId.Value == clientId;

    }

    public abstract bool TryUpgrade(ulong clientId);

}
