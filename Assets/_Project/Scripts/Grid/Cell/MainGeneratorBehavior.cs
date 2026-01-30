using UnityEngine;
using Unity.Netcode;

public class MainGeneratorBehavior : UpgradableBehavior
{
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return; // Only the server handles claiming

        var player = other.GetComponent<PlayerState>();
        if (player == null) return;

        if (player.OwnedRoomId.Value != -1)
            return; // Player already owns a room

        bool success = TryClaimRoom(player.OwnerClientId);
        if (success)
        {
            player.OwnedRoomId.Value = roomId;
            Debug.Log($"Room {roomId} successfully claimed by client {player.OwnerClientId}");
        }
        else
        {
            Debug.Log($"Room {roomId} claim by client {player.OwnerClientId} failed");
        }
    }

    bool TryClaimRoom(ulong clientId)
    {
        var grid = Object.FindFirstObjectByType<GridManager>();
        if (grid == null) return false;

        bool success = grid.TryClaimRoom(roomId, clientId);

        if (success)
        {
            ownerClientId.Value = clientId;
        }
        return success;
    }


    
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void ClaimRoomServerRpc(ulong clientId)
    {
        if (HasOwner) return;

        ownerClientId.Value = clientId;
    }
}
