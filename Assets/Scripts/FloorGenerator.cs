using UnityEngine;
using UnityEngine.Tilemaps;

public class GridGenerator : MonoBehaviour{

    IRoomPositionProvider roomProvider;

    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    public TileBase floorTile;
    public TileBase wallTile;

    const int FLOOR_SIZE = 32;
    const int WALL_THICKNESS = 2;

    const int ROOM_SIZE = 6;
    const int ROOM_WALL_THICKNESS = 1;
    const int ROOM_COUNT = 9;

    const int CORRIDOR_WIDTH = 2;

    void Start()
    {
        // roomProvider = new RandomRoomProvider();
        roomProvider = new FixedGridRoomProvider();
        int totalSize = FLOOR_SIZE + WALL_THICKNESS * 2;
        int[,] map = new int[totalSize, totalSize];

        GenerateMapData(map);
        GenerateRooms(map);
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

    bool CanPlaceRoom(int[,] map, int startX, int startY)
    {
        int roomTotalSize = ROOM_SIZE + ROOM_WALL_THICKNESS * 2;
        int checkSize = roomTotalSize + CORRIDOR_WIDTH;

        for (int y = 0; y < checkSize; y++)
        {
            for (int x = 0; x < checkSize; x++)
            {
                if (map[startY + y, startX + x] != 1)
                    return false;
            }
        }
        return true;
    }


    void PlaceRoom(int[,] map, int startX, int startY)
    {
        int roomTotalSize = ROOM_SIZE + 2;

        for (int y = 0; y < roomTotalSize; y++)
        {
            for (int x = 0; x < roomTotalSize; x++)
            {
                bool isWall =
                    x == 0 || y == 0 ||
                    x == roomTotalSize - 1 ||
                    y == roomTotalSize - 1;

                map[startY + y, startX + x] = isWall ? 4 : 3;
            }
        }
    }

    void GenerateRooms(int[,] map)
    {
        var positions = roomProvider.GetRoomPositions(
            map.GetLength(0),
            ROOM_COUNT,
            ROOM_SIZE,
            WALL_THICKNESS,
            CORRIDOR_WIDTH
        );

        foreach (var p in positions)
        {
            if (CanPlaceRoom(map, p.x, p.y))
                PlaceRoom(map, p.x, p.y);
        }
    }

    void PaintMap(int[,] map)
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
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

                else if (map[y, x] == 3)
                    floorTilemap.SetTile(pos, floorTile);

                else if (map[y, x] == 4)
                    wallTilemap.SetTile(pos, wallTile);
            }
        }
    }
}