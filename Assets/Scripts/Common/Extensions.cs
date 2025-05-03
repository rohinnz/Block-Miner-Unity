using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyExtensions
{
    public static Vector3Int Vector3Int(this Vector2Int v2)
    {
        return new Vector3Int(v2.x, v2.y);
    }
    public static Vector2Int Vector2Int(this Vector3Int v3)
    {
        return new Vector2Int(v3.x, v3.y);
    }

}
