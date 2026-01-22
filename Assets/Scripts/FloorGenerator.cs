using UnityEngine;
using UnityEngine.Tilemaps;

public class GridGenerator : MonoBehaviour
{
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    public TileBase floorTile;
    public TileBase wallTile;

    const int FLOOR_SIZE = 30;
    const int WALL_THICKNESS = 2;

    void Start()
    {
        int totalSize = FLOOR_SIZE + WALL_THICKNESS * 2;
        int[,] map = new int[totalSize, totalSize];

        GenerateMapData(map);
        PaintMap(map);
    }

    void GenerateMapData(int[,] map)
    {
        int size = map.GetLength(0);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool isWall =
                    x < WALL_THICKNESS ||
                    x >= size - WALL_THICKNESS ||
                    y < WALL_THICKNESS ||
                    y >= size - WALL_THICKNESS;

                map[y, x] = isWall ? 2 : 1;
            }
        }
    }

    void PaintMap(int[,] map)
    {
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                int offset = map.GetLength(0) / 2;
                Vector3Int pos = new Vector3Int(x - offset, offset - y, 0);

                if (map[y, x] == 1)
                    floorTilemap.SetTile(pos, floorTile);

                else if (map[y, x] == 2)
                    wallTilemap.SetTile(pos, wallTile);
            }
        }
    }
}