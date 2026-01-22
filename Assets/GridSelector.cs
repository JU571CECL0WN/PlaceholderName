using UnityEngine;
using UnityEngine.InputSystem;

public class GridSelector : MonoBehaviour
{
    public GridManager grid;
    public Transform highlight; // un cuadrado visual

    void Update()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

        Vector2Int cell = grid.WorldToCell(mouseWorld);
        highlight.position = grid.CellToWorld(cell);
    }
}