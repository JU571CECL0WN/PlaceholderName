using UnityEngine;
using UnityEngine.Tilemaps;

public class Highlight : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Grid grid;

    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    public void Initialize(Grid grid){
        this.grid = grid;
    }

    public void Show(Vector3Int cellPosition){
        Vector3 worldPos = grid.CellToWorld(cellPosition);
        transform.position = worldPos;
        spriteRenderer.enabled = true;
    }

    public void Hide(){
        spriteRenderer.enabled = false;
    }
}