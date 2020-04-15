using UnityEngine;
using UnityEngine.Tilemaps;

public class MapUtils
{
    public static Vector3Int? FindTopTile(Tilemap tileMap, int x)
    {
        for (var i = tileMap.cellBounds.yMax; i >= 0; i--)
        {
            var pos = new Vector3Int(x, i, 0);
            var tile = tileMap.GetTile(pos);
            if (tile != null) return pos;
        }

        return null;
    }
}