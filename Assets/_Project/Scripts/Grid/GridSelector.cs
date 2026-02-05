using UnityEngine;
using UnityEngine.InputSystem;

public class GridSelector : MonoBehaviour
{
    [SerializeField] private GridManager grid;
    [SerializeField] private GameObject highlightPrefab;

    private Transform highlight;

    public void Initialize(GridManager gridManager)
    {
        grid = gridManager;
        highlight = Instantiate(highlightPrefab).transform;
        highlight.gameObject.SetActive(false);
    }

    void Update()
    {
        if (highlight == null || grid == null)
            return;

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

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log($"Clicked cell: {cell.x}, {cell.y}, is type: {grid.GetCell(cell)?.type}");
        }
    }
}