using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GeneralEnemyAI : NetworkBehaviour
{
    public Transform target;
    public float speed = 3f;
    public PathFinding pathFinding;

    private List<Vector3> path = new List<Vector3>();
    private int pathIndex = 0;
    private float timer = 0f;

    void Update()
    {
        if (!IsServer || target == null || pathFinding == null) return;


        timer += Time.deltaTime;
        if (timer > 0.5f)
        {
            timer = 0f;
            path = pathFinding.FindPath(transform.position, target.position);
            pathIndex = 0;
        }

        if (pathIndex >= path.Count) return;

        Vector3 dir = (path[pathIndex] - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, path[pathIndex]) < 0.2f)
            pathIndex++;
    }
}
