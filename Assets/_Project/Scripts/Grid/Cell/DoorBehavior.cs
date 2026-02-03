using Unity.Netcode;
using UnityEngine;

public class DoorBehavior : BreakableBehavior
{
    [SerializeField] private BoxCollider2D solidCollider;
    [SerializeField] private BoxCollider2D triggerCollider;

    public override void OnNetworkSpawn()
    {
        solidCollider = GetComponent<BoxCollider2D>();
        triggerCollider = GetComponentInChildren<BoxCollider2D>();

        if (IsServer)
        {
            hitPoints.Value = 10;
        }

        hitPoints.OnValueChanged += OnHitPointsChanged;
        OnHitPointsChanged(0, hitPoints.Value);
    }

    public override void OnNetworkDespawn()
    {
        hitPoints.OnValueChanged -= OnHitPointsChanged;
    }

    private void OnHitPointsChanged(int oldValue, int newValue)
    {
        bool enabled = newValue > 0;
        solidCollider.enabled = enabled;
        triggerCollider.enabled = enabled;
    }

    public void HandleTriggerEnter(Collider2D collider)
    {
        if (!collider.TryGetComponent<PlayerState>(out var player))
            return;

        bool shouldIgnore = ShouldIgnorePlayer(player);

        Physics2D.IgnoreCollision(
            solidCollider,
            collider,
            shouldIgnore
        );
    }


    public void HandleTriggerExit(Collider2D collider)
    {
        if (!collider.TryGetComponent<PlayerState>(out var player))
            return;

        Physics2D.IgnoreCollision(
            solidCollider,
            collider,
            false
        );
    }

    private bool ShouldIgnorePlayer(PlayerState player)
    {
        return
        player.OwnedRoomId.Value == -1 ||
        player.OwnedRoomId.Value == roomId.Value;
    }
}