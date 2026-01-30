using System;
using Unity.Netcode;
using UnityEngine;

public class UpgradableBehavior : NetworkBehaviour
{
    public virtual int roomId { get; set; }

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

}
