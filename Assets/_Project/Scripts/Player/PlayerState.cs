using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerState : NetworkBehaviour
{
    public NetworkVariable<int> money = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // Room que posee el player (-1 = ninguna)
    public NetworkVariable<int> OwnedRoomId = new NetworkVariable<int>(-1);

    public NetworkVariable<bool> isSleeping = new(false);

    // Generador activo mientras duerme
    public NetworkVariable<ulong> activeGeneratorNetId =
        new NetworkVariable<ulong>(
            ulong.MaxValue,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    private Coroutine sleepCoroutine;

    [SerializeField] private int passiveMoneyPerTick = 0;

    /* =========================
     *  NETWORK LIFECYCLE
     * ========================= */

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        money.Value = 0;
        sleepCoroutine = StartCoroutine(MoneyTick());
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        if (sleepCoroutine != null)
        {
            StopCoroutine(sleepCoroutine);
            sleepCoroutine = null;
        }
    }

    /* =========================
     *  GENERATOR INTERACTION
     * ========================= */

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestGeneratorInteractionServerRpc(ulong generatorNetId)
    {
        if (isSleeping.Value) return;

        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects
            .TryGetValue(generatorNetId, out var netObj))
            return;

        var gen = netObj.GetComponent<MainGeneratorBehavior>();
        if (gen == null) return;

        gen.TryClaimGenerator(OwnerClientId, this);
    }

    /* =========================
     *  SLEEP / WAKE
     * ========================= */

    public void SetSleepingServer(bool sleeping, ulong generatorNetId = ulong.MaxValue)
    {
        if (!IsServer) return;

        isSleeping.Value = sleeping;
        activeGeneratorNetId.Value = sleeping ? generatorNetId : ulong.MaxValue;
    }

    [ClientRpc]
    public void ConfirmSleepClientRpc(
        Vector3 bedPosition,
        ClientRpcParams rpcParams = default
    )
    {
        if (!IsOwner) return;

        transform.position = bedPosition;

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    [Rpc(SendTo.Server)]
    public void RequestWakeUpServerRpc()
    {
        if (!isSleeping.Value) return;
        WakeUp();
    }

    private void WakeUp()
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
    private void ConfirmWakeUpClientRpc(ClientRpcParams rpcParams = default)
    {
        if (!IsOwner) return;

        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    /* =========================
     *  MONEY TICK (SERVER ONLY)
     * ========================= */

    private IEnumerator MoneyTick()
    {
        while (true)
        {
            money.Value += passiveMoneyPerTick;

            if (isSleeping.Value && activeGeneratorNetId.Value != ulong.MaxValue)
            {
                if (NetworkManager.Singleton.SpawnManager.SpawnedObjects
                    .TryGetValue(activeGeneratorNetId.Value, out var netObj))
                {
                    var gen = netObj.GetComponent<MainGeneratorBehavior>();
                    Debug.Log($"[INCOME] Using Generator NetId = {activeGeneratorNetId.Value}");
                    if (gen != null)
                    {
                        int income = gen.GetMoneyPerTick();
                        money.Value += income;
                        Debug.Log("+" + income + " from generator while sleeping.");
                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    /* =========================
     *  MISC
     * ========================= */

    public void SetOwnedRoomServer(int roomId)
    {
        if (!IsServer) return;
        OwnedRoomId.Value = roomId;
    }

    public void AddPassiveIncomeServer(int amount)
    {
        if (!IsServer) return;
        passiveMoneyPerTick += amount;
    }
}