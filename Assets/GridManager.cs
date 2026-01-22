using UnityEngine;

public class GridManager : MonoBehaviour
{
    public float cellSize = 1f;

    public Vector2Int WorldToCell(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        return new Vector3(
            cell.x * cellSize + cellSize / 2f,
            cell.y * cellSize + cellSize / 2f,
            0
        );
    }
}