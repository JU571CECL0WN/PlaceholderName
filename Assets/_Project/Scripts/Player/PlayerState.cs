using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerState : NetworkBehaviour
{
    public NetworkVariable<int> money = new(0);

    // roomId en la que está el player (-1 = ninguna)
    public NetworkVariable<int> OwnedRoomId = new NetworkVariable<int>(-1);

    public NetworkVariable<bool> isSleeping = new(false);

    private Coroutine sleepCoroutine;

    [SerializeField] private int moneyPerTick = 1;


    public override void OnNetworkSpawn()
    {
        Debug.Log(
            $"[Player] LocalClient={NetworkManager.Singleton.LocalClientId} " +
            $"OwnerId={OwnerClientId} IsOwner={IsOwner}"
        );
        if (IsServer)
        {
            isSleeping.OnValueChanged += OnSleepingChanged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            isSleeping.OnValueChanged -= OnSleepingChanged;
        }
    }

    public void SleepAt(Vector3 position)
    {
        if (!IsServer) return;

        isSleeping.Value = true;

        transform.position = position;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }

    private void OnSleepingChanged(bool oldValue, bool newValue)
    {
        if (!IsServer) return;

        if (newValue)
        {
            sleepCoroutine = StartCoroutine(SleepTick());
        }
        else
        {
            if (sleepCoroutine != null)
            {
                StopCoroutine(sleepCoroutine);
                sleepCoroutine = null;
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void RequestWakeUpServerRpc()
    {
        if (!isSleeping.Value) return;

        WakeUp();
    }

    public void WakeUp()
    {
        if (!IsServer) return;

        isSleeping.Value = false;
    }

    private IEnumerator SleepTick()
    {
        while (isSleeping.Value)
        {
            money.Value += moneyPerTick;

            yield return new WaitForSeconds(1f);
        }
    }
}