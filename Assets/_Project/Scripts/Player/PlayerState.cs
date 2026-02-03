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


    public void SetOwnedRoomServer(int roomId)
    {
        if (!IsServer) return;
        OwnedRoomId.Value = roomId;
    }


    public override void OnNetworkSpawn()
    {
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

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestGeneratorClaimServerRpc(ulong generatorNetId)
    {
        if (isSleeping.Value) return;

        var gen = NetworkManager.Singleton.SpawnManager
            .SpawnedObjects[generatorNetId]
            .GetComponent<MainGeneratorBehavior>();

        if (gen == null) return;

        gen.TryClaimGenerator(OwnerClientId, this);
    }

    [ClientRpc]
    public void ConfirmSleepClientRpc(
        Vector3 bedPosition,
        ClientRpcParams rpcParams = default
    )
    {
        if (!IsOwner) return;
        
        transform.position = bedPosition;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void SetSleepingServer(bool sleeping)
    {
        if (!IsServer) return;

        isSleeping.Value = sleeping;
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

        ConfirmWakeUpClientRpc(
        new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new[] { OwnerClientId }
            }
        }
    );

        SetSleepingServer(false);
    }

    [ClientRpc]
    public void ConfirmWakeUpClientRpc(ClientRpcParams rpcParams = default)
    {
        if (!IsOwner) return;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
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