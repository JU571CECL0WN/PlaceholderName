using UnityEngine;
using Unity.Netcode;

public class MainGeneratorBehavior : UpgradableBehavior
{
    // Fuente ÚNICA de income
    protected NetworkVariable<int> moneyPerTick =
        new NetworkVariable<int>(
            1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        moneyPerTick.OnValueChanged += (oldValue, newValue) =>
        {
            Debug.Log(
                $"Money per tick changed from {oldValue} to {newValue} on {name}"
            );
        };
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerState>(out var player)) return;
        if (!player.IsOwner) return;

        player.RequestGeneratorInteractionServerRpc(NetworkObjectId);
    }

    public int GetMoneyPerTick()
    {
        return moneyPerTick.Value;
    }

    public void TryClaimGenerator(ulong clientId, PlayerState player)
    {
        if (!IsServer) return;

        if (ownerClientId.Value == clientId)
        {
            GoToSleep(player);
            return;
        }

        if (ownerClientId.Value != ulong.MaxValue)
            return;

        var grid = Object.FindFirstObjectByType<GridManager>();
        if (grid == null) return;

        if (!grid.TryClaimRoom(roomId.Value, clientId))
            return;

        ownerClientId.Value = clientId;
        player.SetOwnedRoomServer(roomId.Value);

        GoToSleep(player);
    }

    private void GoToSleep(PlayerState player)
    {
        if (!IsServer) return;

        player.ConfirmSleepClientRpc(
            transform.position,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { player.OwnerClientId }
                }
            }
        );

        player.SetSleepingServer(true, NetworkObjectId);
    }

    protected override int GetUpgradeCost()
    {
        if (baseUpgradeCost == int.MaxValue)
            baseUpgradeCost = 10;

        return baseUpgradeCost * (level.Value + 1);
    }

    protected override void ApplyUpgrade()
    {
        moneyPerTick.Value += 1;
    }
}