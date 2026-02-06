using UnityEngine;

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

    // TODO: No seria mala idea hacer esto configurable desde unity, osea hacerlo serializable

    int nextRoomId = 0;

    const int FLOOR_SIZE = 32;
    const int WALL_THICKNESS = 2;

    const int ROOM_SIZE = 6;
    const int ROOM_WALL_THICKNESS = 1;
    const int ROOM_COUNT = 7;

    const int CORRIDOR_WIDTH = 2;

    public CellData[,] Generate(){
        roomProvider = CreateRoomProvider();
        nextRoomId = 0;

        int totalSize = FLOOR_SIZE + WALL_THICKNESS * 2;
        CellData[,] map = new CellData[totalSize, totalSize];

        GenerateMapData(map);
        GenerateRooms(map);

        return map;
    }

    void GenerateMapData(CellData[,] map)
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

                map[y, x] = new CellData { type = isWall ? CellType.Wall : CellType.Floor ,
                roomId = -1};
            }
        }
    }

    bool CanPlaceRoom(CellData[,] map, int startX, int startY)
    {
        int roomTotalSize = ROOM_SIZE + ROOM_WALL_THICKNESS * 2;
        int checkSize = roomTotalSize + CORRIDOR_WIDTH;

        int size = map.GetLength(0);
        if (startX + checkSize >= size || startY + checkSize >= size)
            return false;

        for (int y = 0; y < checkSize; y++)
        {
            for (int x = 0; x < checkSize; x++)
            {
                if (map[startY + y, startX + x].type != CellType.Floor)
                    return false;
            }
        }

        return true;
    }


    void PlaceRoom(CellData[,] map, int startX, int startY, int roomId)
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

                map[startY + y, startX + x] = new CellData {
                    type = isWall ? CellType.RoomWall : CellType.RoomFloor,
                    roomId = roomId
                    };
            }
        }
    }

    void PlaceDoorway(CellData[,] map, int startX, int startY, int roomId)
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

        map[mapY, mapX] = new CellData { 
            type = CellType.Door,
            roomId = roomId
            };
    }

    void PlaceMainGenerator(CellData[,] map, int startX, int startY, int roomId)
    {
        int roomTotalSize = ROOM_SIZE + ROOM_WALL_THICKNESS * 2;

        int min = 1;
        int max = roomTotalSize - 3;

        int localX = Random.Range(min, max + 1);
        int localY = Random.Range(min, max + 1);

        map[startY + localY, startX + localX] = new CellData
        {
            type = CellType.MainGenerator, // o Bed
            roomId = roomId
        };
    }

    void GenerateRooms(CellData[,] map)
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
                int roomId = nextRoomId++;

                PlaceRoom(map, p.x, p.y, roomId);
                PlaceDoorway(map, p.x, p.y, roomId);
                PlaceMainGenerator(map, p.x, p.y, roomId);
            }
        }
    }
}