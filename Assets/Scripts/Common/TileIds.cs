using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UIElements;

[Serializable]
public struct TileId
{
    public TileBase Tile;
    public TileType Id;
}

[CreateAssetMenu(fileName = "TileIds", menuName = "Block Miner/New TileIds", order = 10)]
public class TileIds : ScriptableObject
{
    public const int EmptyId = 0;
    public TileId[] Ids;

    public Dictionary<TileBase, int> GetTileToIdDict()
    {
        Dictionary<TileBase, int> m_tileToId = new Dictionary<TileBase, int>(Ids.Length);
        foreach (var tileId in Ids)
        {
            m_tileToId.Add(tileId.Tile, (int)tileId.Id);
        }

        return m_tileToId;
    }

    public Dictionary<int, TileBase> GetIdToTileDict()
    {
        Dictionary<int, TileBase> m_tileToId = new Dictionary<int, TileBase>(Ids.Length);
        foreach (var tileId in Ids)
        {
            m_tileToId.Add((int)tileId.Id, tileId.Tile);
        }

        return m_tileToId;
    }
}
