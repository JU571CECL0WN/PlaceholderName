using UnityEngine;
using UnityEngine.InputSystem;

public class GridSelector : MonoBehaviour
{
    public GridManager grid;
    public Transform highlight;

    void Start()
    {
        highlight.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Mouse.current == null || !Application.isFocused)
        {
            highlight.gameObject.SetActive(false);
            return;
        }

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0;

        Vector2Int cell = grid.WorldToCell(mouseWorld);

        highlight.gameObject.SetActive(true);
        highlight.position = grid.CellToWorld(cell);
    }
}