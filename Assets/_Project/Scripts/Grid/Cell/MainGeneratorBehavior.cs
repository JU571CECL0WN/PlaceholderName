using UnityEngine;
using Unity.Netcode;

public class MainGeneratorBehavior : NetworkBehaviour
{
    public NetworkVariable<int> roomId =
        new NetworkVariable<int>(
            -1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

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
        if (!other.TryGetComponent<PlayerState>(out var player)) return;

        if (!player.IsOwner) return;

        player.RequestGeneratorClaimServerRpc(NetworkObjectId);
    }

    public void TryClaimGenerator(ulong clientId, PlayerState player)
    {
        if (!IsServer) return;

        if (ownerClientId.Value != ulong.MaxValue &&
            ownerClientId.Value != clientId)
            return;

        var grid = Object.FindFirstObjectByType<GridManager>();
        if (grid == null) return;

        if (!grid.TryClaimRoom(roomId.Value, clientId))
            return;

        ownerClientId.Value = clientId;
        
        player.ConfirmSleepClientRpc(
            transform.position,
            new ClientRpcParams {
                Send = new ClientRpcSendParams {
                    TargetClientIds = new[] { clientId }
                }
            }
        );

        player.SetSleepingServer(true);
    }
}
