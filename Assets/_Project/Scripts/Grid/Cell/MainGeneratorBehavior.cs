using UnityEngine;
using Unity.Netcode;

public class MainGeneratorBehavior : NetworkBehaviour
{
    public int roomId;

    private NetworkVariable<ulong> ownerClientId =
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

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void ClaimRoomServerRpc(ulong clientId)
    {
        if (HasOwner) return;

        ownerClientId.Value = clientId;
        Debug.Log($"Room {roomId} claimed by client {clientId}");
    }
}
