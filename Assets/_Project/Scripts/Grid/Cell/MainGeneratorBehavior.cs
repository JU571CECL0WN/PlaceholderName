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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return; // Only the server handles claiming

        var player = other.GetComponent<PlayerState>();

        if (player == null) return;

        // Try to claim the room if the player doesn't own any room
        if (player.OwnedRoomId.Value == -1)
        {
            bool success = TryClaimRoom(player.OwnerClientId);
            if (success)
            {
                player.OwnedRoomId.Value = roomId;
                Debug.Log($"Room {roomId} successfully claimed by client {player.OwnerClientId}");
            }
            else
                Debug.Log($"Room {roomId} claim by client {player.OwnerClientId} failed");
        }

        // If the player owns this room, let them sleep
        if (player.OwnerClientId == ownerClientId.Value)
        {
            player.SleepAt(transform.position);
            Debug.Log($"Player {player.OwnedRoomId.Value} sleeping in room {roomId}");
        }
    }

    bool TryClaimRoom(ulong playerId)
    {
        var grid = Object.FindFirstObjectByType<GridManager>();
        if (grid == null) return false;

        bool success = grid.TryClaimRoom(roomId, playerId);

        if (success)
        {
            ownerClientId.Value = playerId;
        }
        return success;
    }
}
