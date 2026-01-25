using UnityEngine;
using UnityEngine.Tilemaps;

enum WallSide
{
    Top,
    Bottom,
    Left,
    Right
}

public class GridGenerator : MonoBehaviour{

    IRoomPositionProvider roomProvider;
    IRoomPositionProvider CreateRoomProvider()
    {
        switch (roomProviderType)
        {
            case RoomProviderType.Fixed:
                return new FixedGridRoomProvider();

            case RoomProviderType.Random:
                return new RandomRoomProvider();

            default:
                Debug.LogError("Unknown RoomProviderType");
                return new FixedGridRoomProvider();
        }
    }

    public RoomProviderType roomProviderType;

    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap doorTilemap;

    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase doorTile;

    const int FLOOR_SIZE = 32;
    const int WALL_THICKNESS = 2;

    const int ROOM_SIZE = 6;
    const int ROOM_WALL_THICKNESS = 1;
    const int ROOM_COUNT = 7;

    const int CORRIDOR_WIDTH = 2;

    void Start()
    {
        // roomProvider = new RandomRoomProvider();
        roomProvider = CreateRoomProvider();
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
        int roomTotalSize = ROOM_SIZE + ROOM_WALL_THICKNESS * 2;

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

    void PlaceDoorway(int[,] map, int startX, int startY)
    {
        int roomTotalSize = ROOM_SIZE + ROOM_WALL_THICKNESS * 2;

        WallSide side = (WallSide)Random.Range(0, 4);

        int min = 1;
        int max = roomTotalSize - 2;

        int localX = 0;
        int localY = 0;

        switch (side)
        {
            case WallSide.Top:
                localX = Random.Range(min, max);
                localY = roomTotalSize - 1;
                break;

            case WallSide.Bottom:
                localX = Random.Range(min, max);
                localY = 0;
                break;

            case WallSide.Left:
                localX = 0;
                localY = Random.Range(min, max);
                break;

            case WallSide.Right:
                localX = roomTotalSize - 1;
                localY = Random.Range(min, max);
                break;
        }

        int mapX = startX + localX;
        int mapY = startY + localY;

        map[mapY, mapX] = 5; // DOOR
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
            {
                PlaceRoom(map, p.x, p.y);
                PlaceDoorway(map, p.x, p.y);
            }
        }
    }

    void PaintMap(int[,] map)
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        doorTilemap.ClearAllTiles();
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

                else if (map[y, x] == 5)
                    doorTilemap.SetTile(pos, doorTile);
            }
        }
    }
}