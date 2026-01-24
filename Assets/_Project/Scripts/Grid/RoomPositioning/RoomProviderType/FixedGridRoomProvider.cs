using System.Collections.Generic;
using UnityEngine;

public class FixedGridRoomProvider : IRoomPositionProvider
{
    public List<Vector2Int> GetRoomPositions(
        int mapSize,
        int roomCount,
        int roomSize,
        int wallThickness,
        int roomPadding)
    {
        List<Vector2Int> result = new();

        int roomTotalSize = roomSize + 2; // paredes de la habitación
        int blockSize = roomTotalSize + roomPadding;

        int min = wallThickness + roomPadding;
        int max = mapSize - wallThickness - roomTotalSize - roomPadding;

        for (int y = min; y <= max; y += blockSize)
        {
            for (int x = min; x <= max; x += blockSize)
            {
                result.Add(new Vector2Int(x, y));

                if (result.Count >= roomCount)
                    return result;
            }
        }

        return result;
    }
}