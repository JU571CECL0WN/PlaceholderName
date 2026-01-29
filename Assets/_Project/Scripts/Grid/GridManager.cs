using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Netcode;

public class GridManager : NetworkBehaviour
{
    [Header("Grid")]
    [SerializeField] float cellSize = 1f;

    [Header("Tilemaps")]
    [SerializeField] Tilemap floorTilemap;
    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap doorTilemap;

    [Header("Tiles")]
    [SerializeField] TileBase floorTile;
    [SerializeField] TileBase wallTile;
    [SerializeField] TileBase doorTile;

    [SerializeField] GameObject mainGeneratorPrefab;

    CellData[,] map;
    Dictionary<int, RoomData> rooms = new();

    //============================================
    // Getters / Setters
    //============================================

    void Awake()
    {
        Debug.Log("GridManager Awake");
    }

    void Start()
    {
        Debug.Log("GridManager Start");
    }

    public void SetMap(CellData[,] generatedMap)
    {
        map = generatedMap;
        BuildRooms();
        PaintMap();
    }

    public CellData GetCell(Vector2Int cell)
    {
        if (cell.y < 0 || cell.y >= map.GetLength(0) ||
            cell.x < 0 || cell.x >= map.GetLength(1))
            return null;

        return map[cell.y, cell.x];
    }

    public RoomData GetRoom(int roomId)
    {
        if (rooms.TryGetValue(roomId, out var room))
            return room;

        return null;
    }

    //============================================
    // Public Methods
    //============================================

    public Vector2Int WorldToCell(Vector2 worldPos)
    {
        int sizeX = map.GetLength(1);
        int offset = sizeX / 2;

        int x = Mathf.FloorToInt(worldPos.x / cellSize) + offset;
        int y = offset - Mathf.FloorToInt(worldPos.y / cellSize);

        return new Vector2Int(x, y);
    }

    public Vector3 CellToWorld(Vector2Int cell)
    {
        int sizeX = map.GetLength(1);
        int offset = sizeX / 2;

        return new Vector3(
            (cell.x - offset) * cellSize + cellSize / 2f,
            (offset - cell.y) * cellSize + cellSize / 2f,
            0
        );
    }
    
    bool generatorsSpawned = false;

    public override void OnNetworkSpawn()
    {
        Debug.Log("GridManager OnNetworkSpawn");

        if (!IsServer) return;
        if (map == null)
        {
            Debug.Log("Map is null, cannot spawn generators");
            return;
        }

        if (generatorsSpawned) return;

        SpawnMainGenerators();
        generatorsSpawned = true;
    }

    //============================================
    // Private Methods
    //============================================


    void BuildRooms()
    {
        rooms.Clear();

        int sizeY = map.GetLength(0);
        int sizeX = map.GetLength(1);

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                var cell = map[y, x];
                if (cell.roomId < 0)
                    continue;

                if (!rooms.TryGetValue(cell.roomId, out var room))
                {
                    room = new RoomData { roomId = cell.roomId };
                    rooms.Add(cell.roomId, room);
                }

                Vector2Int pos = new(x, y);
                room.cells.Add(pos);

                if (cell.type == CellType.Door)
                    room.doors.Add(pos);
            }
        }
    }

    void SpawnMainGenerators()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Not server, not spawning generators");
            return;
        }

        int sizeY = map.GetLength(0);
        int sizeX = map.GetLength(1);

        int count = 0;

        for (int y = 0; y < sizeY; y++)
        for (int x = 0; x < sizeX; x++)
        {
            if (map[y, x].type == CellType.MainGenerator)
            count++;

            if (map[y, x].type != CellType.MainGenerator)
                continue;

            Vector3 worldPos = CellToWorld(new Vector2Int(x, y));
            
            var gen = Instantiate(mainGeneratorPrefab, worldPos, Quaternion.identity);

            gen.GetComponent<MainGeneratorBehavior>().roomId = map[y, x].roomId;
            gen.GetComponent<NetworkObject>().Spawn();
        }
    }

    public bool TryClaimRoom(int roomId, ulong clientId)
    {
        if (!rooms.TryGetValue(roomId, out var room))
            return false;

        if (room.ownerClientId != ulong.MaxValue)
            return room.ownerClientId == clientId;

        room.ownerClientId = clientId;
        return true;
    }

    void PaintMap()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        doorTilemap.ClearAllTiles();

        int sizeY = map.GetLength(0);
        int sizeX = map.GetLength(1);
        int offset = sizeX / 2;

        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                Vector3Int pos = new Vector3Int(x - offset, offset - y, 0);

                switch (map[y, x].type)
                {
                    case CellType.Floor:
                    case CellType.RoomFloor:
                        floorTilemap.SetTile(pos, floorTile);
                        break;

                    case CellType.Wall:
                    case CellType.RoomWall:
                        wallTilemap.SetTile(pos, wallTile);
                        break;

                    case CellType.Door:
                        doorTilemap.SetTile(pos, doorTile);
                        break;

                    case CellType.MainGenerator:
                        floorTilemap.SetTile(pos, floorTile);
                        break;
                }
            }
        }
    }
}