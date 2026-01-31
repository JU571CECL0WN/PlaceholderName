using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Netcode;

public class WorldInitializer : NetworkBehaviour
{
    [SerializeField] GridManager gridManagerPrefab;
    [SerializeField] GridGenerator gridGeneratorPrefab;
    [SerializeField] GridSelector gridSelectorPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        var gridGenerator = Instantiate(gridGeneratorPrefab);
        var gridManager = Instantiate(gridManagerPrefab);
        gridManager.GetComponent<NetworkObject>().Spawn();
    }
}