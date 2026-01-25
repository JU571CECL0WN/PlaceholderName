using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinding : MonoBehaviour
{
    public Tilemap wallTilemap;

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        Vector3Int startCell = wallTilemap.WorldToCell(start);
        Vector3Int endCell = wallTilemap.WorldToCell(end);

        var openSet = new List<Vector3Int> { startCell };
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var gScore = new Dictionary<Vector3Int, float> { [startCell] = 0 };

        while (openSet.Count > 0)
        {
            // Get cell with lowest score
            Vector3Int current = openSet[0];
            float lowestF = gScore[current] + Heuristic(current, endCell);
            foreach (var cell in openSet)
            {
                float f = gScore[cell] + Heuristic(cell, endCell);
                if (f < lowestF) { current = cell; lowestF = f; }
            }

            if (current == endCell)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            // Check neighbors (4 directions)
            Vector3Int[] dirs = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };
            foreach (var dir in dirs)
            {
                Vector3Int neighbor = current + dir;
                if (wallTilemap.HasTile(neighbor)) continue; // blocked

                float tentativeG = gScore[current] + 1;
                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Vector3>(); // no path
    }

    float Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    List<Vector3> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        var path = new List<Vector3> { wallTilemap.GetCellCenterWorld(current) };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(wallTilemap.GetCellCenterWorld(current));
        }
        path.Reverse();
        return path;
    }
}
