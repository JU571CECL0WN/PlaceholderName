using UnityEngine;
using Unity.Netcode;

public class MainGeneratorBehavior : UpgradableBehavior
{

    private NetworkVariable<int> moneyPerTick = new NetworkVariable<int>(1);

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
            
        player.ConfirmSleepClientRpc(
            transform.position,
            new ClientRpcParams {
                Send = new ClientRpcSendParams {
                    TargetClientIds = new[] { clientId }
                }
            }
        );
        player.setActiveIncome(moneyPerTick.Value);
        player.SetSleepingServer(true);
    }

    public override bool TryUpgrade(ulong clientId)
    {
        if (!IsServer) return false;

        if (ownerClientId.Value != clientId) return false;

        moneyPerTick.Value += 1;

        return true;
    }
}
