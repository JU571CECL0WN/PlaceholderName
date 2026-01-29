using Unity.Netcode;
using UnityEngine;

public class PlayerState : NetworkBehaviour
{
    public NetworkVariable<int> money = new(0);

    // roomId en la que está el player (-1 = ninguna)
    public NetworkVariable<int> OwnedRoomId = new NetworkVariable<int>(-1);

    public NetworkVariable<bool> isSleeping = new(false);
}