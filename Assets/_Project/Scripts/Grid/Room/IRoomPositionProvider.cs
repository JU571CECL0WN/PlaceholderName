using System.Collections.Generic;
using UnityEngine;

public interface IRoomPositionProvider
{
    List<Vector2Int> GetRoomPositions(
        int mapSize,
        int roomCount,
        int roomSize,
        int wallThickness,
        int roomPadding
    );
}