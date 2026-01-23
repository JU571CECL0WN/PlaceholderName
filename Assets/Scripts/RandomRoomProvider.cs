using System.Collections.Generic;
using UnityEngine;

public class RandomRoomProvider : IRoomPositionProvider
{
    public List<Vector2Int> GetRoomPositions(
        int mapSize,
        int roomCount,
        int roomSize,
        int wallThickness,
        int roomPadding)
    {
        List<Vector2Int> result = new();

        int blockSize = roomSize + roomPadding;
        int min = wallThickness + roomPadding;
        int max = mapSize - wallThickness - blockSize;

        List<Vector2Int> candidates = new();

        for (int y = min; y <= max; y += blockSize)
        {
            for (int x = min; x <= max; x += blockSize)
            {
                candidates.Add(new Vector2Int(x, y));
            }
        }

        if (candidates.Count < roomCount)
        {
            Debug.LogError($"Not enough room slots. Needed {roomCount}, but got {candidates.Count}");
            return result;
        }

        for (int i = 0; i < candidates.Count; i++)
        {
            int j = Random.Range(i, candidates.Count);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        for (int i = 0; i < roomCount; i++)
            result.Add(candidates[i]);

        return result;
    }
}