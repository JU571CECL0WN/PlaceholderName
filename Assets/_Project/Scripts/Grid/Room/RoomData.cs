using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RoomData
{
    public int roomId;

    public ulong ownerClientId = ulong.MaxValue; 
    
    public List<Vector2Int> cells = new();
    public List<Vector2Int> upgradableCells = new();

    public bool IsOwned => ownerClientId != ulong.MaxValue;

    public bool hasTile(Vector2Int tilePos){
        return cells.Contains(tilePos);
    }
}