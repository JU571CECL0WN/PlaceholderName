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

    public override void OnNetworkSpawn()
    {
        if (IsServer){
            var generator = Object.FindFirstObjectByType<GridGenerator>();
            Debug.Log("GridManager OnNetworkSpawn found generator: " + (generator != null));
            CellData[,] generatedMap = generator.Generate();

            map = generatedMap;
            BuildRooms();
            SpawnMainGenerators();

            PaintMap();
            }

        if (!IsServer)
        {
            RequestMapServerRpc(); // Request map from server on client connect
        }

    }

    //============================================
    // Private Methods
    //============================================

    CellDataNet[] SerializeMap(CellData[,] map)
    {
        int height = map.GetLength(0);
        int width = map.GetLength(1);

        CellDataNet[] data = new CellDataNet[height * width + 2];

        data[0] = new CellDataNet { roomId = height };
        data[1] = new CellDataNet { roomId = width };

        int i = 2;

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            data[i++] = new CellDataNet
            {
                type = (byte)map[y, x].type,
                roomId = map[y, x].roomId
            };
        }

        return data;
    }

    CellData[,] DeserializeMap(CellDataNet[] data)
    {
        int height = data[0].roomId;
        int width = data[1].roomId;

        CellData[,] map = new CellData[height, width];

        int i = 2;

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            map[y, x] = new CellData
            {
                type = (CellType)data[i].type,
                roomId = data[i].roomId
            };
            i++;
        }

        return map;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void RequestMapServerRpc()
    {
        SendMapClientRpc(SerializeMap(map));
    }


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

    [ClientRpc]
    void SendMapClientRpc(CellDataNet[] data)
    {
        if (IsServer) return;

        map = DeserializeMap(data);

        PaintMap();
    }

    void PaintMap()
    {
        Debug.Log("Painting map");
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