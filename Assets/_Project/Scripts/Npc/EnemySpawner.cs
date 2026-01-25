using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public Vector3 spawnPosition;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.GetComponent<NetworkObject>().Spawn();
    }
}

